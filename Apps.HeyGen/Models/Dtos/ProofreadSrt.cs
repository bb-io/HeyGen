using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class ProofreadSrt
{
    [JsonProperty("srt_url")]
    public string Url { get; set; } = string.Empty;
}
