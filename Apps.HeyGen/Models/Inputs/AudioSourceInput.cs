using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.HeyGen.Models.Inputs;

public class AudioSourceInput
{
    [Display("Audio file", Description = "A WAV or MP3 file from Blackbird.")]
    public FileReference? AudioFile { get; set; }

    [Display("Audio URL", Description = "An HTTPS URL to an audio file HeyGen can download directly.")]
    public string? AudioUrl { get; set; }

    [Display("Audio asset ID", Description = "The ID of an audio asset already available in HeyGen.")]
    public string? AudioAssetId { get; set; }
}
