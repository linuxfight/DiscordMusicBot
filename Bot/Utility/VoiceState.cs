using System.Diagnostics;
using Discord.Audio;

namespace DiscordMusicBot.Bot.Utility;

public class VoiceState
{
    public bool Connected { get; set; }
    public bool Looped { get; set; }
    public List<Song> Songs { get; set; } = new();
    public IAudioClient? AudioClient { get; set; }
    private AudioOutStream? _discordAudio;

    public async Task Stop()
    {
        if (_discordAudio != null)
        {
            await _discordAudio.DisposeAsync();
            _discordAudio = null;
        }

        if (AudioClient != null)
        {
            await AudioClient.StopAsync();
            AudioClient = null;
        }
        Connected = false;
        Looped = false;
        Songs = new();
    }

    public async Task Skip()
    {
        CancellationTokenSource cts = new();
        Songs.RemoveAt(0);
        
        if (_discordAudio != null)
        {
            await _discordAudio.ClearAsync(cts.Token);
            await _discordAudio.DisposeAsync();
            _discordAudio = null;
        }

        if (Songs.Count == 0)
            await Stop();
        else
            await PlayMusic();
    }

    public async Task PlayMusic()
    {
        while (true)
        {
            Song current = Songs.First();
            string? audioUrl = await CobaltApiClient.Json(current.YoutubeUrl);
            if (audioUrl == null) return;
            using (Process? ffmpeg = CreateStream(audioUrl))
            await using (Stream output = ffmpeg!.StandardOutput.BaseStream)
            await using (_discordAudio = AudioClient?.CreatePCMStream(AudioApplication.Mixed))
            {
                try
                {
                    await output.CopyToAsync(_discordAudio!);
                }
                finally
                {
                    await _discordAudio!.FlushAsync();
                    if (!Looped) Songs.RemoveAt(0);
                }
            }
            
            if (Songs.Count >= 1)
                continue;
            await Stop();
            break;
        }
    }

    private Process? CreateStream(string url)
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-hide_banner -loglevel panic -i \"{url}\" -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        });
    }
}

public class Song
{
    public required string Title { get; init; }
    public required string Artist { get; init; }
    public required string YoutubeUrl { get; init; }
}