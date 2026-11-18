using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class ErrorEnvelope
{
    [JsonProperty("error")]
    public ErrorDetail? Error { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }
}
