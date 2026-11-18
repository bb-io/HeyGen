using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class ScriptSelectionInput : VideoInput
{
    [Display("Script entry ID")]
    [DataSource(typeof(ScriptEntryHandler))]
    public string? ScriptEntryId { get; set; }

    [Display("Source language tag")]
    public string SourceLanguage { get; set; } = string.Empty;

    [Display("Target language tag")]
    public string TargetLanguage { get; set; } = string.Empty;
}
