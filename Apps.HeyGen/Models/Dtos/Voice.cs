using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class Voice
{
    [JsonProperty("voice_id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("language")]
    public string? Language { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; }

    [JsonProperty("mode")]
    public string? Mode { get; set; }
}
