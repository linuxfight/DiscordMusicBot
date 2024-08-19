using Discord.WebSocket;

namespace DiscordMusicBot.Bot;

public abstract class Command
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Func<SocketSlashCommand, Task> Handler { get; set; }
}