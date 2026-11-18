using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class Asset
{
    [JsonProperty("asset_id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}
