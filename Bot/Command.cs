using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.Bot;

public abstract class Command()
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<Parameter>? Parameters { get; set; }
    public Func<SocketSlashCommand, IServiceProvider, Task>? Handler { get; set; }
}

public class Parameter
{
    public string? Name { get; set; }
    public ApplicationCommandOptionType Type { get; set; }
    public string? Description { get; set; }
    public bool Required { get; set; }
}