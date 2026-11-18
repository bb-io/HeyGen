using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class VideoSearchInput
{
    [Display("Title contains")]
    public string? Title { get; set; }

    [Display("Status")]
    [StaticDataSource(typeof(VideoStatusHandler))]
    public string? Status { get; set; }

    [Display("Maximum results")]
    public int? Maximum { get; set; }
}
