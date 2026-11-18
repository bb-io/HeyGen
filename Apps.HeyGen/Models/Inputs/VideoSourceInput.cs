using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class VideoSourceInput
{
    [Display("Source video file")]
    public FileReference? VideoFile { get; set; }

    [Display("Source video URL")]
    public string? VideoUrl { get; set; }

    [Display("Video asset ID")]
    public string? VideoAssetId { get; set; }

    [Display("Video ID")]
    [DataSource(typeof(VideoHandler))]
    public string? VideoId { get; set; }
}
