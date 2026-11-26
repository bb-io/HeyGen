using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class ClippingInput : VideoSourceInput
{
    [Display("Submission key", Description = "Use a stable unique value for this job. Reuse it only to retry the same job within 24 hours.")]
    public string SubmissionKey { get; set; } = string.Empty;

    [Display("Title")]
    public string? Title { get; set; }

    [Display("Source language code")]
    [StaticDataSource(typeof(SourceLanguageHandler))]
    public string? InputLanguage { get; set; }

    [Display("Clip aspect ratio")]
    [StaticDataSource(typeof(ClipAspectHandler))]
    public string? AspectRatio { get; set; }

    [Display("Target clip lengths")]
    [StaticDataSource(typeof(ClipDurationHandler))]
    public IEnumerable<string>? Durations { get; set; }

    [Display("Burn captions into clips")]
    public bool? Captions { get; set; }

    [Display("Editorial guidance")]
    public string? Prompt { get; set; }

    [Display("Correlation ID")]
    public string? CallbackId { get; set; }
}
