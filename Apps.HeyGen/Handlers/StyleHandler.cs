using Apps.HeyGen.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class StyleHandler(InvocationContext invocationContext)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var items = await Client.Paginate<Style>(() => new RestRequest("video-agents/styles"),
            Picker.Limit, Picker.Limit, x => !string.IsNullOrWhiteSpace(x.Id) && Picker.Matches(context.SearchString, x.Id, x.Name, x.AspectRatio), Picker.Pages);
        return items.DistinctBy(x => x.Id).Select(x => Picker.Item(x.Id, x.Name, x.AspectRatio));
    }
}
