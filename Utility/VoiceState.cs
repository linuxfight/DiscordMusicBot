using System.Diagnostics;
using Discord.Audio;
using DiscordMusicBot.Utility.Cobalt;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.Utility;

public class VoiceState
{
    public bool Connected { get; set; }
    public bool Looped { get; set; }
    public List<Song> Songs { get; set; } = new();
    public IAudioClient? AudioClient { get; set; }
    private AudioOutStream? _discordAudio;
    private Process? _ffmpeg;
    private Stream? _ffmpegMusic;

    public async Task Stop()
    {
        await AudioClient?.StopAsync()!;
        Connected = false;
        Songs = new();
        AudioClient = null;
    }

    public async Task Skip()
    {
        Songs.RemoveAt(0);
        if (_discordAudio != null)
        {
            await _discordAudio.FlushAsync();
            await _discordAudio.DisposeAsync();
            _discordAudio = null;
        }

        if (_ffmpegMusic != null)
        {
            await _ffmpegMusic.FlushAsync();
            await _ffmpegMusic.DisposeAsync();
            _ffmpegMusic = null;
        }

        if (_ffmpeg != null)
        {
            _ffmpeg.Kill();
            await _ffmpeg.WaitForExitAsync();
            _ffmpeg = null;
        }
    }

    public async Task PlayMusic(IServiceProvider serviceProvider)
    {
        while (true)
        {
            Song current = Songs.First();
            CobaltApiResponse? data = await serviceProvider.GetRequiredService<CobaltApiClient>().Json(current.YoutubeUrl);
            if (data == null) return;
            current.AudioUrl = data.Url;
            await using (_discordAudio = AudioClient!.CreatePCMStream(AudioApplication.Music))
            using (_ffmpeg = CreateStream(Songs.First().AudioUrl!))
            await using (_ffmpegMusic = _ffmpeg!.StandardOutput.BaseStream)
            {
                try
                {
                    await _ffmpegMusic.CopyToAsync(_discordAudio!);
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
    public required string Title { get; set; }
    public required string Artist { get; set; }
    public required string YoutubeUrl { get; set; }
    public string? AudioUrl { get; set; }
}