using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Outputs;

public class ClipDetails
{
    [Display("Clip ID")]
    [JsonProperty("id")]
    public string ClipId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string? Title { get; set; }

    [Display("Status")]
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [Display("Duration (seconds)")]
    [JsonProperty("duration_seconds")]
    public double? Duration { get; set; }

    [Display("Virality score")]
    [JsonProperty("virality_score")]
    public double? ViralityScore { get; set; }

    [Display("Video URL")]
    [JsonProperty("video_url")]
    public string? VideoUrl { get; set; }

    [Display("Thumbnail URL")]
    [JsonProperty("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }
}
