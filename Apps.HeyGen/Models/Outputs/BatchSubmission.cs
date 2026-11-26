using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Outputs;

public class BatchSubmission
{
    [Display("Batch ID")]
    [JsonProperty("batch_id")]
    public string BatchId { get; set; } = string.Empty;

    [Display("Batch type")]
    public string BatchType { get; set; } = string.Empty;
}
