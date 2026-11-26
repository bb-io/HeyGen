using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Outputs;

public class ClippingDetails
{
    [Display("Clipping job ID")]
    [JsonProperty("id")]
    public string ClippingJobId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string? Title { get; set; }

    [Display("Status")]
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [Display("Progress")]
    [JsonProperty("progress")]
    public int? Progress { get; set; }

    [Display("Clips")]
    [JsonProperty("clips")]
    public List<ClipDetails> Clips { get; set; } = [];

    [Display("Failure message")]
    [JsonProperty("failure_message")]
    public string? FailureMessage { get; set; }
}
