using System.Text.Json.Serialization;

namespace DiscordMusicBot.Utility;

[Serializable]
public class CobaltApiRequest
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
    [JsonPropertyName("vCodec")]
    public string VideoCodec { get; set; } = "vp9";
    [JsonPropertyName("vQuality")]
    public string VideoQuality { get; set; } = "360";
    [JsonPropertyName("aFormat")]
    public string AudioFormat { get; set; } = "opus";
    [JsonPropertyName("filenamePattern")]
    public string FilenamePattern { get; set; } = "pretty";
    [JsonPropertyName("isAudioOnly")]
    public bool IsAudioOnly { get; set; } = true;
    [JsonPropertyName("isTTFullAudio")]
    public bool TikTokFullAudio { get; set; } = false;
    [JsonPropertyName("isAudioMuted")]
    public bool AudioMuted { get; set; } = false;
    [JsonPropertyName("dubLang")]
    public bool DubLang { get; set; } = false;
    [JsonPropertyName("disableMetadata")]
    public bool DisableMetadata { get; set; } = false;
    [JsonPropertyName("twitterGif")]
    public bool TwitterGif { get; set; } = false;
    [JsonPropertyName("tiktokH265")]
    public bool TiktokH265 { get; set; } = false;
}
    
/*
[Serializable]
public enum FilenamePattern
{
    Classic,
    Pretty,
    Basic,
    Nerdy
}
    
[Serializable]
public enum AudioFormat
{
    Best,
    Mp3,
    Ogg,
    Wav,
    Opus
}

[Serializable]
public enum VideoQuality
{
    144,
    240,
    360,
    480,
    720,
    1080,
    1440,
    2160,
    Max
}
    
[Serializable]
public enum VideoCodec
{
    H264,
    Av1,
    Vp9
}
*/