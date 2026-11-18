using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class CursorPage<T>
{
    [JsonProperty("data")]
    public List<T> Data { get; set; } = [];

    [JsonProperty("has_more")]
    public bool HasMore { get; set; }

    [JsonProperty("next_token")]
    public string? NextToken { get; set; }
}
