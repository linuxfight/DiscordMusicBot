using YoutubeExplode;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;

namespace DiscordMusicBot.Bot.Utility;

public static class YoutubeApiClient
{
    private static readonly YoutubeClient YoutubeClient = new();

    public static async Task<VideoSearchResult?> SearchVideo(string query)
    {
        return await YoutubeClient.Search.GetVideosAsync(query).FirstOrDefaultAsync();
    }

    public static async Task<Video> GetVideoInfo(string url)
    {
        return await YoutubeClient.Videos.GetAsync(url);
    }
}