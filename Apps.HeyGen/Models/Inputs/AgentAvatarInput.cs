using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class AgentAvatarInput : AgentInput
{
    [Display("Avatar group ID")]
    [DataSource(typeof(AvatarGroupHandler))]
    public string? AvatarGroupId { get; set; }

    [Display("Avatar ID")]
    [DataSource(typeof(AvatarHandler))]
    public string? AvatarId { get; set; }
}
