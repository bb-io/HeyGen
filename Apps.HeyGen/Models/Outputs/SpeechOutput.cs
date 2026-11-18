using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.HeyGen.Models.Outputs;

public class SpeechOutput
{
    [Display("Audio file")]
    public FileReference File { get; set; } = default!;

    [Display("Duration (seconds)")]
    public double Duration { get; set; }

    [Display("HeyGen request ID")]
    public string? RequestId { get; set; }

    [Display("Detected or requested language")]
    public string? Language { get; set; }
}
