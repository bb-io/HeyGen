using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class AgentInput
{
    [Display("Prompt")]
    public string Prompt { get; set; } = string.Empty;

    [Display("Voice ID")]
    [DataSource(typeof(VoiceHandler))]
    public string? VoiceId { get; set; }

    [Display("Agent style ID")]
    [DataSource(typeof(StyleHandler))]
    public string? AgentStyleId { get; set; }

    [Display("Brand glossary ID")]
    [DataSource(typeof(GlossaryHandler))]
    public string? BrandGlossaryId { get; set; }

    [Display("Orientation")]
    [StaticDataSource(typeof(OrientationHandler))]
    public string? Orientation { get; set; }

    [Display("Reference files")]
    public List<FileReference>? Files { get; set; } = [];

    [Display("Correlation ID")]
    public string? CallbackId { get; set; }
}
