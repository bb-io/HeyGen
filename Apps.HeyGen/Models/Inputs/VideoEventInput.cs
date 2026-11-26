using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class VideoEventInput
{
    [Display("Video ID")]
    [DataSource(typeof(VideoHandler))]
    public string? VideoId { get; set; }
}
