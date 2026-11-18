using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class GlossaryTerm
{
    [JsonProperty("term")]
    public string Term { get; set; } = string.Empty;

    [JsonProperty("pronunciation")]
    public string? Pronunciation { get; set; }

    [JsonProperty("pronunciation_language")]
    public LanguageScope? PronunciationLanguage { get; set; }

    [JsonProperty("ipa")]
    public string? Ipa { get; set; }

    [JsonProperty("translation")]
    public string? Translation { get; set; }

    [JsonProperty("target_language")]
    public LanguageScope? TargetLanguage { get; set; }
}
