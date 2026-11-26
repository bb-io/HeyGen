using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Outputs;

public class ProofreadRender
{
    [Display("Translation ID")]
    [JsonProperty("video_translation_id")]
    public string TranslationId { get; set; } = string.Empty;

    [Display("Status")]
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;
}
