using YoutubeExplode;
using YoutubeExplode.Search;

namespace DiscordMusicBot.Utility;

public class YoutubeApiClient
{
    private readonly YoutubeClient _client = new();

    public async Task<VideoSearchResult?> SearchVideo(string query)
    {
        return await _client.Search.GetVideosAsync(query).FirstOrDefaultAsync();
    }
}