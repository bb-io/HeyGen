using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Inputs;

public class TemplateExportInput : TemplateInput
{
    [Display("Source language tag")]
    public string SourceLanguage { get; set; } = string.Empty;

    [Display("Target language tag")]
    public string TargetLanguage { get; set; } = string.Empty;
}
