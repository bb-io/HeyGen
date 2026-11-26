using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class ClippingHandler(InvocationContext invocationContext)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var items = await Client.Paginate<ClippingDetails>(() => new RestRequest("ai-clipping"),
            Picker.Limit, Picker.Limit, x => !string.IsNullOrWhiteSpace(x.ClippingJobId) && Picker.Matches(context.SearchString, x.ClippingJobId, x.Title, x.Status), Picker.Pages);
        return items.DistinctBy(x => x.ClippingJobId).Select(x => Picker.Item(x.ClippingJobId, x.Title, x.Status));
    }
}
