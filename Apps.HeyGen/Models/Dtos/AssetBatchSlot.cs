using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class AssetBatchSlot
{
    [JsonProperty("upload_url")]
    public string UploadUrl { get; set; } = string.Empty;

    [JsonProperty("upload_headers")]
    public Dictionary<string, string> Headers { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    [JsonProperty("max_bytes")]
    public long MaximumBytes { get; set; }
}
