using Apps.HeyGen.Api;
using Apps.HeyGen.Models.Inputs;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.HeyGen.Handlers;

public class AgentDraftHandler(InvocationContext invocationContext, [ActionParameter] AgentSessionInput input)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input?.AgentSessionId))
            return [];

        var result = await AgentDrafts.Read(Client, input.AgentSessionId);
        return result.Videos.Where(draft => draft.Status == "completed" &&
                Picker.Matches(context.SearchString, draft.VideoId, draft.Title))
            .Take(Picker.Limit)
            .Select(draft => Picker.Item(draft.VideoId, draft.Title,
                draft.CreatedAt?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)));
    }
}
