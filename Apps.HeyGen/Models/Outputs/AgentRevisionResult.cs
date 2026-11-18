using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Outputs;

public class AgentRevisionResult
{
    [Display("Agent session ID")]
    [JsonProperty("session_id")]
    public string SessionId { get; set; } = string.Empty;

    [Display("Agent run ID")]
    [JsonProperty("run_id")]
    public string RunId { get; set; } = string.Empty;

    [Display("Video ID")]
    [JsonProperty("video_id")]
    public string? VideoId { get; set; }

    [Display("Title")]
    [JsonProperty("title")]
    public string? Title { get; set; }
}
