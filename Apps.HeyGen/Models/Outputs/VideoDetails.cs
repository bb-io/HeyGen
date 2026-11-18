using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Apps.HeyGen.Models.Outputs;

public class VideoDetails
{
    [Display("Video ID")]
    [JsonProperty("id")]
    public string VideoId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string? Title { get; set; }

    [Display("Status")]
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [Display("Video URL")]
    [JsonProperty("video_url")]
    public string? VideoUrl { get; set; }

    [Display("Captioned video URL")]
    [JsonProperty("captioned_video_url")]
    public string? CaptionedVideoUrl { get; set; }

    [Display("SRT URL")]
    [JsonProperty("subtitle_url")]
    public string? SubtitleUrl { get; set; }

    [Display("Thumbnail URL")]
    [JsonProperty("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }

    [Display("HeyGen page URL")]
    [JsonProperty("video_page_url")]
    public string? PageUrl { get; set; }

    [Display("Duration (seconds)")]
    [JsonProperty("duration")]
    public double? Duration { get; set; }

    [Display("Failure code")]
    [JsonProperty("failure_code")]
    public string? FailureCode { get; set; }

    [Display("Failure message")]
    [JsonProperty("failure_message")]
    public string? FailureMessage { get; set; }

    [Display("Created at")]
    [JsonProperty("created_at")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? CreatedAt { get; set; }

    [Display("Completed at")]
    [JsonProperty("completed_at")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? CompletedAt { get; set; }
}
