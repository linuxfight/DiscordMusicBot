using Discord.WebSocket;
using DiscordMusicBot.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.Bot.Commands;

public class SkipCommand : Command
{
    public SkipCommand()
    {
        Name = "skip";
        Description = "skip track";
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
        
        await command.RespondAsync($"skipping {voiceState.Songs.First()}");
        voiceState.Skip();
    }
}