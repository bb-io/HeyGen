using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class CreateTemplateVideoInput : TemplateRenderInput
{
    [Display("Text variable name")]
    [DataSource(typeof(TextVariableHandler))]
    public string? TextVariable { get; set; }

    [Display("Replacement text")]
    public string? Text { get; set; }

    [Display("Additional variable replacements (JSON)")]
    public string? Variables { get; set; }
}
