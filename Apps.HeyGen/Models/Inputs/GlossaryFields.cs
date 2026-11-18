using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Inputs;

public class GlossaryFields
{
    [Display("Glossary name")]
    public string? Name { get; set; }

    [Display("Pronunciations (JSON array)")]
    public string? Terms { get; set; }

    [Display("Do-not-translate rules (JSON array)")]
    public string? DoNotTranslate { get; set; }

    [Display("Forced translations (JSON array)")]
    public string? ForcedTranslations { get; set; }
}
