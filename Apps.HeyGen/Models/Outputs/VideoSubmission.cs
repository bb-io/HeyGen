using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Outputs;

public class VideoSubmission
{
    [Display("Video ID")]
    [JsonProperty("video_id")]
    public string VideoId { get; set; } = string.Empty;

    [Display("Status")]
    [JsonProperty("status")]
    public string? Status { get; set; }
}
