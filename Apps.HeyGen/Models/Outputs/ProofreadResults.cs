using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Outputs;

public class ProofreadResults
{
    [Display("Ready proofread sessions")]
    public List<ProofreadDetails> Sessions { get; set; } = [];
}
