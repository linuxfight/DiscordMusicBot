using System.Diagnostics;
using Discord.Audio;

namespace DiscordMusicBot.Utility;

public class VoiceState
{
    public bool Connected { get; set; }
    public List<Song> Songs { get; set; } = new();
    public IAudioClient? AudioClient { get; set; }
    public Process? Ffmpeg { get; set; }
    public Stream? Music { get; set; }
    public AudioOutStream? Discord { get; set; }

    public async Task Stop()
    {
        
        await AudioClient!.StopAsync();
        Connected = false;
        Songs = new();
        AudioClient = null;
        Ffmpeg!.Close();
        Ffmpeg = null;
        Music!.Close();
        Music = null;
        Discord!.Close();
        Discord = null;
    }

    public void Skip()
    {
        Ffmpeg!.Close();
        Ffmpeg = null;
        Music!.Close();
        Music = null;
        Discord!.Close();
        Discord = null;
        Songs.RemoveAt(0);
    }
    
    public async Task PlayMusic()
    {
        using (Ffmpeg = CreateStream(Songs.First().Url))
        using (Music = Ffmpeg!.StandardOutput.BaseStream)
        using (Discord = AudioClient!.CreatePCMStream(AudioApplication.Mixed))
        {
            try
            {
                await Music.CopyToAsync(Discord);
            }
            finally
            {
                await Discord.FlushAsync();
                Skip();
            }
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
    public required string Url { get; set; }
}