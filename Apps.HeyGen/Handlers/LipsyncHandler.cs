using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class LipsyncHandler(InvocationContext invocationContext)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var items = await Client.Paginate<LipsyncDetails>(() => new RestRequest("lipsyncs"),
            Picker.Limit, Picker.Limit, x => !string.IsNullOrWhiteSpace(x.LipsyncId) && Picker.Matches(context.SearchString, x.LipsyncId, x.Title, x.Status), Picker.Pages);
        return items.DistinctBy(x => x.LipsyncId).Select(x => Picker.Item(x.LipsyncId, x.Title, x.Status));
    }
}
