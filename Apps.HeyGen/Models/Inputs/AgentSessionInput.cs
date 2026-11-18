using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class AgentSessionInput
{
    [Display("Agent session ID")]
    [DataSource(typeof(AgentSessionHandler))]
    public string AgentSessionId { get; set; } = string.Empty;
}
