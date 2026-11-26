using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class ErrorDetail
{
    [JsonProperty("code")]
    public string? Code { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }
}
