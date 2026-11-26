using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class AvatarLook
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("avatar_type")]
    public string? Type { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; }

    [JsonProperty("supported_api_engines")]
    public List<string> SupportedEngines { get; set; } = [];
}
