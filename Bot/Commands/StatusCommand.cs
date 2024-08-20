using Discord.WebSocket;
using DiscordMusicBot.Utility.Cobalt;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.Bot.Commands;

public class StatusCommand : Command
{
    public StatusCommand()
    {
        Name = "status";
        Description = "Get status of cobalt.tools";
        Handler = Handle;
    }

    private async Task Handle(SocketSlashCommand command, IServiceProvider serviceProvider)
    {
        if (await serviceProvider.GetRequiredService<CobaltApiClient>().Check())
            await command.RespondAsync("API is online");
        else
            await command.RespondAsync("API is offline/blocked");
    }
}