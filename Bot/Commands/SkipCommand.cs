using Discord.WebSocket;
using DiscordMusicBot.Bot.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.Bot.Commands;

public class SkipCommand : Command
{
    public SkipCommand()
    {
        Name = "skip";
        Description = Translation.SkipCommandDescription;
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
            await command.RespondAsync(Translation.Skipping(voiceState.Songs.First()));
            await voiceState.Skip();
        });
        // To add skip we need to change stream to a newer one
        // We need to clear old ffmpeg process and stream and replace them with a new one, and don't interrupt the process of copying bytes to discord stream
        // Or we need a way to close these streams, create new ones and start playing again. Without leaving voice channel
        /* 
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

        await command.RespondAsync($"skipping {voiceState.Songs.First().Title}");
        await voiceState.Skip();
        _ = Task.Run(async () =>
        {
            await voiceState.PlayMusic(serviceProvider);
        });
         */
    }
}