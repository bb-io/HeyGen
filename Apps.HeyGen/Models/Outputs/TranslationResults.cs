using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Outputs;

public class TranslationResults
{
    [Display("Translations")]
    public List<TranslationDetails> Translations { get; set; } = [];
}
