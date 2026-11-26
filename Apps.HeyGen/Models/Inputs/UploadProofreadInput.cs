using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.HeyGen.Models.Inputs;

public class UploadProofreadInput : ProofreadInput
{
    [Display("Edited SRT file", Description = "The approved .srt file from Blackbird.")]
    public FileReference? File { get; set; }

    [Display("Edited SRT URL", Description = "A direct HTTPS URL HeyGen can download without authentication.")]
    public string? Url { get; set; }

    [Display("Edited SRT asset ID", Description = "The ID of a subtitle file already uploaded to HeyGen.")]
    public string? AssetId { get; set; }
}
