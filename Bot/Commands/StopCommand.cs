using Discord.WebSocket;
using DiscordMusicBot.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.Bot.Commands;

public class StopCommand : Command
{
    public StopCommand()
    {
        Name = "stop";
        Description = "stops music and leaves voice channel";
        Handler = Handle;
    }

    private async Task Handle(SocketSlashCommand command, IServiceProvider serviceProvider)
    {
        VoiceState voiceState = serviceProvider.GetRequiredService<VoiceState>();
        if (!voiceState.Connected)
        {
            await command.RespondAsync("already disconnected");
            return;
        }

        if (voiceState.AudioClient == null)
        {
            await command.RespondAsync("audio client is null");
            return;
        }
        
        await voiceState.AudioClient.StopAsync();
        voiceState.Connected = false;
        voiceState.Songs = new();
        voiceState.AudioClient = null;
        await command.RespondAsync("leaving channel");
    }
}