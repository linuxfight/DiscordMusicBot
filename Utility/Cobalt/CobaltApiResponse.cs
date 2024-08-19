using System.Text.Json.Serialization;

namespace DiscordMusicBot.Utility;

public class CobaltApiResponse()
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    [JsonPropertyName("text")]
    public string? Text { get; set; }
    [JsonPropertyName("url")]
    public string? Url { get; set; }
    [JsonPropertyName("pickerType")]
    public string? PickerType { get; set; }
    [JsonPropertyName("picker")]
    public List<CobaltApiResponsePicker>? Picker { get; set; }
    [JsonPropertyName("audio")]
    public string? Audio { get; set; }
}

public class CobaltApiResponsePicker()
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    [JsonPropertyName("url")]
    public string? Url { get; set; }
    [JsonPropertyName("thumb")]
    public string? Thumb { get; set; }
}