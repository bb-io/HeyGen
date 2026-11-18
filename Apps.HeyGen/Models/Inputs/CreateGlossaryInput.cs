using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Inputs;

public class CreateGlossaryInput
{
    [Display("Submission key", Description = "Use a stable unique value for this job. Reuse it only to retry the same job within 24 hours.")]
    public string SubmissionKey { get; set; } = string.Empty;

    [Display("Glossary name")]
    public string Name { get; set; } = string.Empty;

    [Display("Pronunciations (JSON array)")]
    public string? Terms { get; set; }

    [Display("Do-not-translate rules (JSON array)")]
    public string? DoNotTranslate { get; set; }

    [Display("Forced translations (JSON array)")]
    public string? ForcedTranslations { get; set; }
}
