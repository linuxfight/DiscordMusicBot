using System.Text.Json.Serialization;

namespace DiscordMusicBot.Utility.Cobalt;

public class CobaltApiResponse
{
    [JsonPropertyName("status")]
    public string? Status { get; init; }
    [JsonPropertyName("text")]
    public string? Text { get; init; }
    [JsonPropertyName("url")]
    public string? Url { get; init; }
    [JsonPropertyName("pickerType")]
    public string? PickerType { get; init; }
    [JsonPropertyName("picker")]
    public List<CobaltApiResponsePicker>? Picker { get; init; }
    [JsonPropertyName("audio")]
    public string? Audio { get; set; }
}

public class CobaltApiResponsePicker
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    [JsonPropertyName("url")]
    public string? Url { get; set; }
    [JsonPropertyName("thumb")]
    public string? Thumb { get; set; }
}