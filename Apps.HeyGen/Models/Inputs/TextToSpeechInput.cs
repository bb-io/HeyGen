using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Models.Inputs;

public class TextToSpeechInput
{
    [Display("Text", Description = "Up to 10,000 characters of text to speak.")]
    public string Text { get; set; } = string.Empty;

    [Display("Voice ID")]
    [DataSource(typeof(VoiceHandler))]
    public string VoiceId { get; set; } = string.Empty;

    [Display("Language", Description = "Optional base language code such as en or hu. Leave blank for detection.")]
    [StaticDataSource(typeof(SpeechLanguageHandler))]
    public string? Language { get; set; }

    [Display("Audio format", Description = "Defaults to WAV.")]
    [StaticDataSource(typeof(SpeechFormatHandler))]
    public string? Format { get; set; }
}
