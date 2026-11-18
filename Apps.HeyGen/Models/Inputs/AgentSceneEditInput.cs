using Blackbird.Applications.Sdk.Common;

namespace Apps.HeyGen.Models.Inputs;

public class AgentSceneEditInput
{
    [Display("Scene ID")]
    public string SceneId { get; set; } = string.Empty;

    [Display("Instruction")]
    public string Instruction { get; set; } = string.Empty;
}
