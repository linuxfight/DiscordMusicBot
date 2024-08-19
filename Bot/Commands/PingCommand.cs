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
    
    public static async Task Handle(SocketSlashCommand command)
    {
        await command.RespondAsync("pong!");
    }
}