using Apps.HeyGen.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Models.Inputs;

public class ProofreadInput
{
    [Display("Proofread session ID")]
    [DataSource(typeof(ProofreadHandler))]
    public string ProofreadId { get; set; } = string.Empty;
}
