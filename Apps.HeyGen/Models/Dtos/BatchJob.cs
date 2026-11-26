using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class BatchJob
{
    [JsonProperty("item_index")]
    public int Index { get; set; }

    [JsonProperty("video_id")]
    public string? JobId { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("error")]
    public ErrorDetail? Error { get; set; }
}
