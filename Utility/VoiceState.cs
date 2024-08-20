using Discord.Audio;

namespace DiscordMusicBot.Utility;

public class VoiceState
{
    public bool Connected { get; set; }
    public List<string> Songs { get; set; } = new();
    public IAudioClient? AudioClient { get; set; }
}