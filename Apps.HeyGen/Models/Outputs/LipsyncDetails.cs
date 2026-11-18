using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Outputs;

public class LipsyncDetails
{
    [Display("Lipsync ID")]
    [JsonProperty("id")]
    public string LipsyncId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string? Title { get; set; }

    [Display("Status")]
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [Display("Video URL")]
    [JsonProperty("video_url")]
    public string? VideoUrl { get; set; }

    [Display("Caption URL")]
    [JsonProperty("caption_url")]
    public string? CaptionUrl { get; set; }

    [Display("Correlation ID")]
    [JsonProperty("callback_id")]
    public string? CallbackId { get; set; }

    [Display("Failure code")]
    [JsonProperty("failure_code")]
    public string? FailureCode { get; set; }

    [Display("Failure message")]
    [JsonProperty("failure_message")]
    public string? FailureMessage { get; set; }
}
