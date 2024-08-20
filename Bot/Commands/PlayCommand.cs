using System.Diagnostics;
using Discord;
using Discord.Audio;
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
        CobaltApiResponse? data;
        if (isLink)
        {
            data = await cobaltApiClient.Json(firstUriPart);
            Video info = await youtubeApiClient.GetVideoInfo(firstUriPart);
            await command.Channel.SendMessageAsync($"[{info.Author.ChannelTitle} - {info.Title}]({info.Url})");
        }
        else
        {
            VideoSearchResult? info = await youtubeApiClient.SearchVideo(video);
            if (info == null)
            {
                await command.Channel.SendMessageAsync("can't find video");
                return;
            }
            await command.Channel.SendMessageAsync($"[{info.Author.ChannelTitle} - {info.Title}]({info.Url})");
            data = await cobaltApiClient.Json(info.Url);
        }

        if (data == null)
        {
            await command.Channel.SendMessageAsync("error, can't get data");
            return;
        }

        VoiceState voiceState = serviceProvider.GetRequiredService<VoiceState>();

        if (voiceState.Connected)
        {
            voiceState.Songs.Add(data.Url!);
            return;
        }
        
        voiceState.AudioClient = await channel.ConnectAsync(disconnect: false, selfDeaf: true);
        await PlayMusic(voiceState, data.Url!);
        if (voiceState.Songs.Count == 1)
        {
            await voiceState.AudioClient.StopAsync();
            voiceState.Connected = false;
            voiceState.Songs = new();
            voiceState.AudioClient = null;
        }
        else
        {
            voiceState.Songs.RemoveAt(0);
            await PlayMusic(voiceState, voiceState.Songs.First());
        }
        await command.RespondAsync("leaving channel");
    }

    private async Task PlayMusic(VoiceState voiceState, string url)
    {
        using (Process? ffmpeg = CreateStream(url))
        using (Stream music = ffmpeg!.StandardOutput.BaseStream)
        using (AudioOutStream discord = voiceState.AudioClient!.CreatePCMStream(AudioApplication.Mixed))
        {
            try
            {
                await music.CopyToAsync(discord);
            }
            finally
            {
                await discord.FlushAsync();
            }
        }
    }
    
    private Process? CreateStream(string url)
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-hide_banner -loglevel panic -i \"{url}\" -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        });
    }
}