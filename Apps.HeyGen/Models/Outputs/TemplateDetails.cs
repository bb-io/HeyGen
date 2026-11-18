using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Outputs;

public class TemplateDetails
{
    [Display("Template ID")]
    public string TemplateId { get; set; } = string.Empty;

    [Display("Template name")]
    public string Name { get; set; } = string.Empty;

    [Display("Variable names, types and defaults (JSON)")]
    public string Variables { get; set; } = string.Empty;
}
