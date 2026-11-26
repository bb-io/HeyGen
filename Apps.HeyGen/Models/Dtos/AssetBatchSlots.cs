using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class AssetBatchSlots
{
    [JsonProperty("batch_id")]
    public string BatchId { get; set; } = string.Empty;

    [JsonProperty("items")]
    public List<AssetBatchSlot> Items { get; set; } = [];
}
