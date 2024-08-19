using YoutubeExplode;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;

namespace DiscordMusicBot.Utility;

public class YoutubeApiClient
{
    private readonly YoutubeClient _youtubeClient = new();
    private readonly HttpClient _httpClient = new();

    public async Task<VideoSearchResult?> SearchVideo(string query)
    {
        return await _youtubeClient.Search.GetVideosAsync(query).FirstOrDefaultAsync();
    }

    public async Task<Video> GetVideoInfo(string url)
    {
        return await _youtubeClient.Videos.GetAsync(url);
    }
    
    public async Task<string> Download(string url, string fileName)
    {
        string path = Path.Combine(Path.GetTempPath(), fileName);
        
        using (Stream stream = await _httpClient.GetStreamAsync(url))
        {
            using (FileStream fileStream = new(path, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }
        }

        return path;
    }
}