using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class TranslateInput : VideoSourceInput
{
    [Display("Submission key", Description = "Use a stable unique value for this job. Reuse it only to retry the same job within 24 hours.")]
    public string SubmissionKey { get; set; } = string.Empty;

    [Display("Target languages")]
    [DataSource(typeof(LanguageHandler))]
    public List<string> Languages { get; set; } = [];

    [Display("Title")]
    public string? Title { get; set; }

    [Display("Mode")]
    [StaticDataSource(typeof(QualityHandler))]
    public string? Mode { get; set; }

    [Display("Translate audio only")]
    public bool? AudioOnly { get; set; }

    [Display("Brand glossary ID")]
    [DataSource(typeof(GlossaryHandler))]
    public string? BrandGlossaryId { get; set; }

    [Display("Correlation ID")]
    public string? CallbackId { get; set; }
}
