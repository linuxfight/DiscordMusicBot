using Discord.WebSocket;
using DiscordMusicBot.Bot.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.Bot.Commands;

public class LoopCommand : Command
{
    public LoopCommand()
    {
        Name = "loop";
        Description = Translation.LoopCommandDescription;
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

        voiceState.Looped = !voiceState.Looped;
        await command.RespondAsync(Translation.Looping(voiceState.Looped));
    }
}