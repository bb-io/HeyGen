using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class AgentSessionHandler(InvocationContext invocationContext)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var items = await Client.Paginate<AgentDetails>(() => new RestRequest("video-agents"),
            Picker.Limit, Picker.Limit, x => !string.IsNullOrWhiteSpace(x.AgentSessionId) && Picker.Matches(context.SearchString, x.AgentSessionId, x.Title), Picker.Pages);
        return items.DistinctBy(x => x.AgentSessionId).Select(x => Picker.Item(x.AgentSessionId, x.Title));
    }
}
