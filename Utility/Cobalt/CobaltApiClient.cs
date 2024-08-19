using System.Text;
using System.Text.Json;

namespace DiscordMusicBot.Utility;

public class CobaltApiClient
{
    private readonly HttpClient _client;
    //private JsonSerializerOptions _serializerOptions;
    private const string UserAgent = "DiscordMusicBot (https://github.com/linuxfight/DiscordMusicBot)";
    private const string ApplicationJson = "application/json";

    public CobaltApiClient(string baseUrl)
    {
        _client = new();
        _client.BaseAddress = new Uri(baseUrl);
        _client.DefaultRequestHeaders.Add("Accept", ApplicationJson);
        _client.DefaultRequestHeaders.Add("User-Agent", UserAgent);

        /*
        _serializerOptions = new()
        {
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        */
    }

    public async Task<bool> Check()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/serverInfo");
        return response.IsSuccessStatusCode;
    }

    public async Task<CobaltApiResponse?> Json(string url)
    {
        CobaltApiRequest request = new()
        {
            Url = url
        };
        string json = JsonSerializer.Serialize(request);
        HttpResponseMessage response = await _client.PostAsync("/api/json", new StringContent(json, Encoding.UTF8, 
            ApplicationJson));
        string content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CobaltApiResponse>(content) ?? null;
    }
}