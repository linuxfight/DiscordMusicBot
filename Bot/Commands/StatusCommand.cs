using Discord.WebSocket;

namespace DiscordMusicBot.Bot.Commands;

public class StatusCommand : Command
{
    public StatusCommand()
    {
        Name = "status";
        Description = "Get status of cobalt.tools";
        Handler = Handle;
    }

    public async Task Handle(SocketSlashCommand command)
    {
        HttpClient client = new();
        HttpResponseMessage response = await client.GetAsync("https://api.cobalt.tools/api/serverInfo");
        if (response.IsSuccessStatusCode)
            await command.RespondAsync("API is online");
        else
            await command.RespondAsync("API is offline/blocked");
    }
}