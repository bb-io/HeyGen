using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class GlossaryInput
{
    [Display("Brand glossary ID")]
    [DataSource(typeof(GlossaryHandler))]
    public string BrandGlossaryId { get; set; } = string.Empty;
}
