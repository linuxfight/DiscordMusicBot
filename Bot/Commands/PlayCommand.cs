using Discord;
using Discord.WebSocket;
using DiscordMusicBot.Bot.Utility;
using Microsoft.Extensions.DependencyInjection;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;

namespace DiscordMusicBot.Bot.Commands;

public class PlayCommand : Command
{
    public PlayCommand()
    {
        Name = "play";
        Description = Translation.PlayCommandDescription;
        Handler = Handle;
        Parameters =
        [
            new()
            {
                Name = Translation.TrackParamName,
                Required = true,
                Description = Translation.TrackParamDescription,
                Type = ApplicationCommandOptionType.String
            }
        ];
    }
    
    private async Task Handle(SocketSlashCommand command, IServiceProvider serviceProvider)
    {
        SocketGuildUser? user = command.User as SocketGuildUser;
        if (user == null)
        {
            await command.RespondAsync("error, user is null");
            return;
        }
        IVoiceChannel? channel = user.VoiceChannel;
        if (channel == null)
        {
            await command.RespondAsync(Translation.UserNotInVoice);
            return;
        }
        
        await command.RespondAsync(Translation.Searching);
        
        string? track = command.Data.Options.First().Value as string;
        if (track == null)
        {
            await command.Channel.SendMessageAsync("url is null");
            return;
        }
        string firstUriPart = track.Split(" ").First();
        bool isLink = Uri.TryCreate(firstUriPart, UriKind.Absolute, out Uri? uriResult) 
                      && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps) && firstUriPart.Contains("youtu");
        
        VoiceState voiceState = serviceProvider.GetRequiredService<VoiceState>();
        if (isLink)
        {
            Video info = await YoutubeApiClient.GetVideoInfo(firstUriPart);
            Song song = new() { Title = info.Title, Artist = info.Author.ChannelTitle, YoutubeUrl = info.Url };
            voiceState.Songs.Add(song);
            await command.Channel.SendMessageAsync(Translation.Track(song));
        }
        else
        {
            VideoSearchResult? info = await YoutubeApiClient.SearchVideo(track);
            if (info == null)
            {
                await command.Channel.SendMessageAsync(Translation.TrackNotFound);
                return;
            }
            Song song = new() { Title = info.Title, Artist = info.Author.ChannelTitle, YoutubeUrl = info.Url };
            voiceState.Songs.Add(song);
            await command.Channel.SendMessageAsync(Translation.Track(song));
        }
        
        if (voiceState.Connected)
        {
            return;
        }

        _ = Task.Run(async () =>
        {
            voiceState.AudioClient = await channel.ConnectAsync(disconnect: false, selfDeaf: true);
            voiceState.Connected = true;
            await voiceState.PlayMusic();
        });
    }
}