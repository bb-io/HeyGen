using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class Template
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("variables")]
    public Dictionary<string,TemplateVariable> Variables { get; set; } = new(StringComparer.Ordinal);
}
