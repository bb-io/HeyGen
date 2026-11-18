using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class TranslationCaptionInput : TranslationInput
{
    [Display("Caption format")]
    [StaticDataSource(typeof(CaptionFormatHandler))]
    public string? Format { get; set; }
}
