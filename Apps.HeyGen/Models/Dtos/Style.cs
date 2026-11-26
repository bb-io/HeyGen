using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class Style
{
    [JsonProperty("style_id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("aspect_ratio")]
    public string? AspectRatio { get; set; }
}
