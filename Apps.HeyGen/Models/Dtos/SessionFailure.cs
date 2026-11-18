using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class SessionFailure
{
    [Display("Failure code")]
    [JsonProperty("code")]
    public string? Code { get; set; }

    [Display("Failure message")]
    [JsonProperty("message")]
    public string? Message { get; set; }
}
