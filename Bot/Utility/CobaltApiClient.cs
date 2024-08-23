using System.Text;
using System.Text.Json;

namespace DiscordMusicBot.Bot.Utility;

public static class CobaltApiClient
{
    private static readonly HttpClient Client = new();
    private const string UserAgent = "DiscordMusicBot (https://github.com/linuxfight/DiscordMusicBot)";
    private const string ApplicationJson = "application/json";
    private const string BaseUrl = "https://api.cobalt.tools";

    static CobaltApiClient()
    {
        Client.BaseAddress = new Uri(BaseUrl);
        Client.DefaultRequestHeaders.Add("Accept", ApplicationJson);
        Client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
    }

    public static async Task<bool> Check()
    {
        HttpResponseMessage response = await Client.GetAsync("/api/serverInfo");
        return response.IsSuccessStatusCode;
    }

    public static async Task<string?> Json(string url)
    {
        Dictionary<string, string> request = new()
        {
            { "url", url },
            { "vCodec", "vp9" },
            { "vQuality", "360" },
            { "aFormat", "opus" },
            { "isAudioOnly", "true" }
        };
        string json = JsonSerializer.Serialize(request);
        HttpResponseMessage responseMessage = await Client.PostAsync("/api/json", new StringContent(json, Encoding.UTF8, 
            ApplicationJson));
        string content = await responseMessage.Content.ReadAsStringAsync();
        Dictionary<string, string>? response = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
        return response?["url"];
    }
}