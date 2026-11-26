using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Outputs;

public class GlossaryDetails
{
    [Display("Brand glossary ID")]
    public string BrandGlossaryId { get; set; } = string.Empty;

    [Display("Glossary name")]
    public string Name { get; set; } = string.Empty;

    [Display("Pronunciations (JSON)")]
    public string Terms { get; set; } = string.Empty;

    [Display("Do-not-translate rules (JSON)")]
    public string DoNotTranslate { get; set; } = string.Empty;

    [Display("Forced translations (JSON)")]
    public string ForcedTranslations { get; set; } = string.Empty;
}
