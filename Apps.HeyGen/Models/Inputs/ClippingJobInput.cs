using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class ClippingJobInput
{
    [Display("Clipping job ID")]
    [DataSource(typeof(ClippingHandler))]
    public string ClippingJobId { get; set; } = string.Empty;
}
