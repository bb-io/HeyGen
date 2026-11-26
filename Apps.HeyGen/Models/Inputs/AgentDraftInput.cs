using Apps.HeyGen.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Models.Inputs;

public class AgentDraftInput : AgentSessionInput
{
    [Display("Draft video ID")]
    [DataSource(typeof(AgentDraftHandler))]
    public string DraftVideoId { get; set; } = string.Empty;
}
