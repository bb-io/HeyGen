using Apps.HeyGen.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Models.Inputs;

public class ProofreadEventInput
{
    [Display("Proofread session ID", Description = "Optional for a workspace trigger. Bind the specific session ID when using a checkpoint.")]
    [DataSource(typeof(ProofreadHandler))]
    public string? ProofreadId { get; set; }
}
