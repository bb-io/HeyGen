using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Inputs;

public class RenderProofreadInput : ProofreadInput
{
    [Display("Submission key", Description = "Use a stable key for this approved revision. Reuse it only for a retry of the same final render within 24 hours.")]
    public string SubmissionKey { get; set; } = string.Empty;
}
