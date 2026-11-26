using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Outputs;

public class ProofreadSubmission
{
    [Display("Proofread session IDs")]
    [JsonProperty("proofread_ids")]
    public List<string> ProofreadIds { get; set; } = [];

    [Display("Status")]
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;
}
