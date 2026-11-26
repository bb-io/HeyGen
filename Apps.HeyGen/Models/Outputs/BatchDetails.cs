using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Outputs;

public class BatchDetails
{
    [Display("Batch ID")]
    public string BatchId { get; set; } = string.Empty;

    [Display("Title")]
    public string? Title { get; set; }

    [Display("Status")]
    public string Status { get; set; } = string.Empty;

    [Display("Total items")]
    public int TotalItems { get; set; }

    [Display("Items")]
    public List<BatchItem> Items { get; set; } = [];
}
