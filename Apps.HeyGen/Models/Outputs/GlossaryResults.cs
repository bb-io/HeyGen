using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Outputs;

public class GlossaryResults
{
    [Display("Brand glossaries")]
    public List<GlossaryDetails> BrandGlossaries { get; set; } = [];
}
