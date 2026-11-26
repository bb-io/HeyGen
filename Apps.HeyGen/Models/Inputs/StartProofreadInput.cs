using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Models.Inputs;

public class StartProofreadInput : VideoSourceInput
{
    [Display("Title")]
    public string Title { get; set; } = string.Empty;

    [Display("Target languages")]
    [DataSource(typeof(LanguageHandler))]
    public List<string> Languages { get; set; } = [];

    [Display("Mode", Description = "Choose Speed or Precision. Defaults to Speed.")]
    [StaticDataSource(typeof(QualityHandler))]
    public string? Mode { get; set; }

    [Display("Brand glossary ID")]
    [DataSource(typeof(GlossaryHandler))]
    public string? BrandGlossaryId { get; set; }

    [Display("Submission key", Description = "Use a stable key for this review request. Reuse it only for the same request within 24 hours.")]
    public string SubmissionKey { get; set; } = string.Empty;
}
