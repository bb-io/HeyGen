using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class ScriptEntry
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("text")]
    public string? Text { get; set; }
}
