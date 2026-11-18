using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Outputs;

public class ClippingSubmission
{
    [Display("Clipping job ID")]
    [JsonProperty("ai_clipping_id")]
    public string ClippingJobId { get; set; } = string.Empty;
}
