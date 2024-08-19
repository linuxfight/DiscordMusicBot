using Discord.WebSocket;

namespace DiscordMusicBot.Bot.Commands;

public class PingCommand : Command
{
    public PingCommand()
    {
        Name = "ping";
        Description = "pong";
        Handler = Handle;
    }

    private static async Task Handle(SocketSlashCommand command, IServiceProvider serviceProvider)
    {
        await command.RespondAsync("pong!");
    }
}