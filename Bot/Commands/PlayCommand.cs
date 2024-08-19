using Discord;
using Discord.WebSocket;
using DiscordMusicBot.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.Bot.Commands;

public class PlayCommand : Command
{
    public PlayCommand()
    {
        Name = "play";
        Description = "Play music from youtube link";
        Handler = Handle;
        Parameters =
        [
            new()
            {
                Name = "link",
                Required = true,
                Description = "youtube link",
                Type = ApplicationCommandOptionType.String
            }
        ];
    }

    private async Task Handle(SocketSlashCommand command, IServiceProvider serviceProvider)
    {
        string? link = command.Data.Options.First().Value as string;

        if (link == null)
        {
            await command.RespondAsync("url is null");
            return;
        }
        CobaltApiResponse? data = await serviceProvider.GetRequiredService<CobaltApiClient>().Json(link);
        if (data == null)
            await command.RespondAsync("error, can't get data");
        else
            await command.RespondAsync(data.Url);
    }
}