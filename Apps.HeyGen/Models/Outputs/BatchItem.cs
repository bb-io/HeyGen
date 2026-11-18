using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Outputs;

public class BatchItem
{
    [Display("Request item index")]
    [JsonProperty("item_index")]
    public int Index { get; set; }

    [Display("Job ID")]
    [JsonProperty("video_id")]
    public string? JobId { get; set; }

    [Display("Status")]
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [Display("Failure message")]
    public string? Failure { get; set; }
}
