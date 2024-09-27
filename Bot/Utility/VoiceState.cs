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
    private CancellationTokenSource _cts = new();

    public async Task Stop()
    {
        if (_discordAudio != null)
            await ClearDiscordAudio();

        Connected = false;
        Looped = false;
        Songs = new();
        if (AudioClient != null)
        {
            await AudioClient.StopAsync();
            AudioClient = null;
        }
    }

    public async Task Skip()
    {
        Songs.RemoveAt(0);

        if (_discordAudio != null)
            await ClearDiscordAudio();
        
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
                    await _discordAudio!.FlushAsync(_cts.Token);
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

    private async Task ClearDiscordAudio()
    {
        await _cts.CancelAsync();
        _cts.Dispose();
        _cts = new();
        if (_discordAudio == null)
            return;
        await _discordAudio.ClearAsync(_cts.Token);
        await _discordAudio.DisposeAsync();
        _discordAudio = null;
    }
}

public class Song
{
    public required string Title { get; init; }
    public required string Artist { get; init; }
    public required string YoutubeUrl { get; init; }
}