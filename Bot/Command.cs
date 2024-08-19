using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.Bot;

public abstract class Command()
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Func<SocketSlashCommand, IServiceProvider, Task> Handler { get; set; }
}