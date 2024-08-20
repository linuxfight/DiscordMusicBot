using YoutubeExplode;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;

namespace DiscordMusicBot.Utility;

public class YoutubeApiClient
{
    private readonly YoutubeClient _youtubeClient = new();

    public async Task<VideoSearchResult?> SearchVideo(string query)
    {
        return await _youtubeClient.Search.GetVideosAsync(query).FirstOrDefaultAsync();
    }

    public async Task<Video> GetVideoInfo(string url)
    {
        return await _youtubeClient.Videos.GetAsync(url);
    }
}