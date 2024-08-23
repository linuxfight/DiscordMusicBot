using Discord.WebSocket;
using DiscordMusicBot.Bot.Utility;

namespace DiscordMusicBot.Bot.Commands;

public class StatusCommand : Command
{
    public StatusCommand()
    {
        Name = "status";
        Description = Translation.StatusCommandDescription;
        Handler = Handle;
    }

    private async Task Handle(SocketSlashCommand command, IServiceProvider serviceProvider)
    {
        if (await CobaltApiClient.Check())
            await command.RespondAsync(Translation.ApiOnline);
        else
            await command.RespondAsync(Translation.ApiOffline);
    }
}