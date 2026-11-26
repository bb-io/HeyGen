using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Outputs;

public class GlossaryDeleted
{
    [Display("Brand glossary ID")]
    public string BrandGlossaryId { get; set; } = string.Empty;
}
