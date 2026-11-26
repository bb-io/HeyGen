using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class BatchPage
{
    [JsonProperty("batch_id")]
    public string BatchId { get; set; } = string.Empty;

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("total_items")]
    public int TotalItems { get; set; }

    [JsonProperty("items")]
    public List<BatchJob> Items { get; set; } = [];

    [JsonProperty("has_more")]
    public bool HasMore { get; set; }

    [JsonProperty("next_token")]
    public string? NextToken { get; set; }
}
