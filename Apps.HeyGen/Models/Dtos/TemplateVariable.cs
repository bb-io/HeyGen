using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.HeyGen.Models.Dtos;

public class TemplateVariable
{
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("content")]
    public string? Content { get; set; }

    [JsonExtensionData]
    public IDictionary<string, JToken>? OtherFields { get; set; }
}
