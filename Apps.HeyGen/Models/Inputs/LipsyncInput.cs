using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class LipsyncInput : VideoSourceInput
{
    [Display("Submission key", Description = "Use a stable unique value for this job. Reuse it only to retry the same job within 24 hours.")]
    public string SubmissionKey { get; set; } = string.Empty;

    [Display("Replacement audio file")]
    public FileReference? AudioFile { get; set; }

    [Display("Replacement audio URL")]
    public string? AudioUrl { get; set; }

    [Display("Audio asset ID")]
    public string? AudioAssetId { get; set; }

    [Display("Title")]
    public string? Title { get; set; }

    [Display("Mode")]
    [StaticDataSource(typeof(QualityHandler))]
    public string? Mode { get; set; }

    [Display("Correlation ID")]
    public string? CallbackId { get; set; }
}
