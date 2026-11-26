using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class SpeechAudio
{
    [JsonProperty("audio_url")]
    public string AudioUrl { get; set; } = string.Empty;

    [JsonProperty("duration")]
    public double Duration { get; set; }
}
