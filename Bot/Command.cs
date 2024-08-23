using Discord;
using Discord.WebSocket;

namespace DiscordMusicBot.Bot;

public abstract class Command
{
    public string? Name { get; protected init; }
    public string? Description { get; protected init; }
    public List<Parameter> Parameters { get; protected init; } = new();
    public Func<SocketSlashCommand, IServiceProvider, Task>? Handler { get; protected init; }
}

public class Parameter
{
    public required string Name { get; init; }
    public ApplicationCommandOptionType Type { get; init; }
    public required string Description { get; init; }
    public bool Required { get; init; }
}