using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Outputs;

public class ScriptOutput
{
    [Display("Translated script")]
    public string Script { get; set; } = string.Empty;

    [Display("Target language tag")]
    public string TargetLanguage { get; set; } = string.Empty;

    [Display("Source video ID")]
    public string VideoId { get; set; } = string.Empty;
}
