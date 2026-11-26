using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class TranslationHandler(InvocationContext invocationContext)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var items = await Client.Paginate<TranslationDetails>(() => new RestRequest("video-translations"),
            Picker.Limit, Picker.Limit, x => !string.IsNullOrWhiteSpace(x.TranslationId) && Picker.Matches(context.SearchString, x.TranslationId, x.Title, x.OutputLanguage), Picker.Pages);
        return items.DistinctBy(x => x.TranslationId).Select(x => Picker.Item(x.TranslationId, x.Title, x.OutputLanguage));
    }
}
