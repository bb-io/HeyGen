using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Outputs;

public class LipsyncSubmission
{
    [Display("Lipsync ID")]
    [JsonProperty("lipsync_id")]
    public string LipsyncId { get; set; } = string.Empty;
}
