using Apps.HeyGen.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Models.Inputs;

public class SpeechToSpeechInput : AudioSourceInput
{
    [Display("Voice ID", Description = "An active instant voice from the connected HeyGen workspace.")]
    [DataSource(typeof(InstantVoiceHandler))]
    public string VoiceId { get; set; } = string.Empty;

    [Display("Remove background noise", Description = "Enable noise removal before conversion.")]
    public bool? RemoveBackgroundNoise { get; set; }

    [Display("Submission key", Description = "Reuse the key with identical inputs to retry the same conversion within 24 hours.")]
    public string SubmissionKey { get; set; } = string.Empty;
}
