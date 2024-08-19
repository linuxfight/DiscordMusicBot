using Discord.WebSocket;

namespace DiscordMusicBot.Bot.Commands;

public class PlayCommand : Command
{
    public PlayCommand()
    {
        Name = "play";
        Description = "Play music";
        Handler = Handle;
    }

    private async Task Handle(SocketSlashCommand command, IServiceProvider serviceProvider)
    {
        await command.RespondAsync("not yet implemented");
    }
}