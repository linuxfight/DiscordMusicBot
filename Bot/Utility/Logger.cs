using Discord;

namespace DiscordMusicBot.Bot.Utility;

public class Logger
{
    public static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}