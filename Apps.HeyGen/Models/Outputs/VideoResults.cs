using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Outputs;

public class VideoResults
{
    [Display("Videos")]
    public List<VideoDetails> Videos { get; set; } = [];
}
