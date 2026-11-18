using Apps.HeyGen.Models.Dtos;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Apps.HeyGen.Models.Outputs;

public class AgentDetails
{
    [Display("Agent session ID")]
    [JsonProperty("session_id")]
    public string AgentSessionId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string? Title { get; set; }

    [Display("Status")]
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [Display("Progress")]
    [JsonProperty("progress")]
    public int? Progress { get; set; }

    [Display("Video ID")]
    [JsonProperty("video_id")]
    public string? VideoId { get; set; }

    [Display("Failure")]
    [JsonProperty("error")]
    public SessionFailure? Failure { get; set; }

    [Display("Created at")]
    [JsonProperty("created_at")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? CreatedAt { get; set; }
}
