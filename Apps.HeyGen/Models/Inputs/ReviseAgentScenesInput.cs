using Apps.HeyGen.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Models.Inputs;

public class ReviseAgentScenesInput : AgentDraftInput
{
    [Display("Edit version", Description = "Map the version returned by List video scenes for the reviewed draft.")]
    public string EditVersion { get; set; } = string.Empty;

    [Display("Scene ID", Description = "Select one scene or map a list to Scene edits.")]
    [DataSource(typeof(AgentSceneHandler))]
    public string? SceneId { get; set; }

    [Display("Instruction")]
    public string? Instruction { get; set; }

    [Display("Scene edits", Description = "An ordered list of scene IDs and instructions. Leave the single-scene fields empty when using this input.")]
    public List<AgentSceneEditInput>? Edits { get; set; }

    [Display("Submission key", Description = "Use a stable key for this approved draft and edit plan.")]
    public string SubmissionKey { get; set; } = string.Empty;
}
