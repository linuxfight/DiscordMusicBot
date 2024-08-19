using Discord;

namespace DiscordMusicBot;

public class Logger
{
    public static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}