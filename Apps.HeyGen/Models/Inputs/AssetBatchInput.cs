using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.HeyGen.Models.Inputs;

public class AssetBatchInput
{
    [Display("Files", Description = "Select 1–100 supported Blackbird files.")]
    public List<FileReference> Files { get; set; } = [];

    [Display("Batch title")]
    public string? Title { get; set; }

    [Display("Submission key", Description = "Reuse the same key and files to resume the same batch within 24 hours.")]
    public string SubmissionKey { get; set; } = string.Empty;
}
