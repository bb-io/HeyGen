using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Outputs;

public class VideoSceneResults
{
    [Display("Draft video ID")]
    public string DraftVideoId { get; set; } = string.Empty;

    [Display("Edit version")]
    public string EditVersion { get; set; } = string.Empty;

    [Display("Scenes")]
    public List<VideoSceneItem> Scenes { get; set; } = [];
}
