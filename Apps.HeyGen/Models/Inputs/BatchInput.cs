using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class BatchInput
{
    [Display("Batch type")]
    [StaticDataSource(typeof(BatchReadTypeHandler))]
    public string BatchType { get; set; } = string.Empty;

    [Display("Batch ID")]
    public string BatchId { get; set; } = string.Empty;
}
