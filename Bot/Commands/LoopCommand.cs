using Discord.WebSocket;
using DiscordMusicBot.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.Bot.Commands;

public class LoopCommand : Command
{
    public LoopCommand()
    {
        Name = "loop";
        Description = "loop current track";
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

        if (voiceState.Looped)
            voiceState.Looped = false;
        else
            voiceState.Looped = true;
        await command.RespondAsync($"loop is now {voiceState.Looped.ToString()}");
    }
}