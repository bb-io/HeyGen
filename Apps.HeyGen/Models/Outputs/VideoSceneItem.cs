using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Outputs;

public class VideoSceneItem
{
    [Display("Scene ID")]
    public string SceneId { get; set; } = string.Empty;

    [Display("Scene number")]
    public int Number { get; set; }

    [Display("Script")]
    public string Script { get; set; } = string.Empty;
}
