using Discord.WebSocket;
using DiscordMusicBot.Bot.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.Bot.Commands;

public class StopCommand : Command
{
    public StopCommand()
    {
        Name = "stop";
        Description = Translation.StopCommandDescription;
        Handler = Handle;
    }

    private async Task Handle(SocketSlashCommand command, IServiceProvider serviceProvider)
    {
        VoiceState voiceState = serviceProvider.GetRequiredService<VoiceState>();
        if (!voiceState.Connected || voiceState.AudioClient == null)
        {
            await command.RespondAsync(Translation.NotConnected);
            return;
        }

        _ = Task.Run(async () =>
        {
            await command.RespondAsync(Translation.Disconnecting);
            await voiceState.Stop();
        });
    }
}