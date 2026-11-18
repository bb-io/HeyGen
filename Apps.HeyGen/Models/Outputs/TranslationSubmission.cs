using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Outputs;

public class TranslationSubmission
{
    [Display("Translation IDs")]
    [JsonProperty("video_translation_ids")]
    public List<string> TranslationIds { get; set; } = [];

    [Display("Warnings")]
    [JsonProperty("warnings")]
    public List<string> Warnings { get; set; } = [];
}
