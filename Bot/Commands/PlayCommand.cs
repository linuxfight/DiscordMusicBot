using Discord;
using Discord.WebSocket;
using DiscordMusicBot.Utility;
using DiscordMusicBot.Utility.Cobalt;
using Microsoft.Extensions.DependencyInjection;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;

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
                Name = "video",
                Required = true,
                Description = "youtube link or title",
                Type = ApplicationCommandOptionType.String
            }
        ];
    }
    
    private async Task Handle(SocketSlashCommand command, IServiceProvider serviceProvider)
    {
        string? video = command.Data.Options.First().Value as string;
        if (video == null)
        {
            await command.RespondAsync("url is null");
            return;
        }
        Uri? uriResult;
        string firstUriPart = video.Split(" ").First();
        bool isLink = Uri.TryCreate(firstUriPart, UriKind.Absolute, out uriResult) 
                      && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps) && firstUriPart.Contains("youtu");
        
        // Get user voice channel
        SocketGuildUser? user = command.User as SocketGuildUser;
        if (user == null)
        {
            await command.RespondAsync("error, user is null");
            return;
        }
        IVoiceChannel? channel = user.VoiceChannel;
        if (channel == null)
        {
            await command.RespondAsync("you are not in voice channel");
            return;
        }
        
        // Get song data
        // WARNING, after this you can only use send message, because interaction is already expired.
        // We need to use this, because without it we'll be terminated for inactivity for 3 seconds. 
        await command.RespondAsync("searching...");
        
        CobaltApiClient cobaltApiClient = serviceProvider.GetRequiredService<CobaltApiClient>();
        YoutubeApiClient youtubeApiClient = serviceProvider.GetRequiredService<YoutubeApiClient>();
        VoiceState voiceState = serviceProvider.GetRequiredService<VoiceState>();
        CobaltApiResponse? data;
        if (isLink)
        {
            data = await cobaltApiClient.Json(firstUriPart);
            Video info = await youtubeApiClient.GetVideoInfo(firstUriPart);
            Song song = new() { Title = info.Title, Artist = info.Author.ChannelTitle, Url = info.Url };
            voiceState.Songs.Add(song);
            await command.Channel.SendMessageAsync($"[{song.Artist} - {song.Title}]({song.Url})");
        }
        else
        {
            VideoSearchResult? info = await youtubeApiClient.SearchVideo(video);
            if (info == null)
            {
                await command.Channel.SendMessageAsync("can't find video");
                return;
            }
            Song song = new() { Title = info.Title, Artist = info.Author.ChannelTitle, Url = info.Url };
            voiceState.Songs.Add(song);
            data = await cobaltApiClient.Json(info.Url);
            await command.Channel.SendMessageAsync($"[{song.Artist} - {song.Title}]({song.Url})");
        }

        if (data == null)
        {
            await command.Channel.SendMessageAsync("error, can't get data");
            return;
        }
        
        if (voiceState.Connected)
        {
            return;
        }
        
        voiceState.AudioClient = await channel.ConnectAsync(disconnect: false, selfDeaf: true);
        voiceState.Connected = true;
        while (voiceState.Songs.Count >= 1)
        {
            await voiceState.PlayMusic();
            voiceState.Songs.RemoveAt(0);
        }
        await voiceState.Stop();
        await command.Channel.SendMessageAsync("leaving channel");
    }
}