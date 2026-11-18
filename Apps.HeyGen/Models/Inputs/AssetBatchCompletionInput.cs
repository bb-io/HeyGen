using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Inputs;

public class AssetBatchCompletionInput
{
    [Display("Batch ID")]
    public string BatchId { get; set; } = string.Empty;
}
