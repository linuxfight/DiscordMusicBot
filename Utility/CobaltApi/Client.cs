namespace DiscordMusicBot.Utility.CobaltApi;

public class Client(string baseUrl)
{
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri(baseUrl)
    };

    public async Task<bool> Check()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/serverInfo");
        return response.IsSuccessStatusCode;
    }
}