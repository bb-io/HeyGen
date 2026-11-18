using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class LanguageScope
{
    [JsonProperty("scope")]
    public string Scope { get; set; } = string.Empty;

    [JsonProperty("code")]
    public string? Code { get; set; }
}
