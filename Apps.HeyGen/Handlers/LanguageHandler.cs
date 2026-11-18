using Apps.HeyGen.Api;
using Apps.HeyGen.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class LanguageHandler(InvocationContext invocationContext)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var languages = await Client.Data<Languages>(new RestRequest("video-translations/languages"));
        return (languages.Available ?? languages.All)
            .Where(name => Requests.Matches(context.SearchString, name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .Select(name => new DataSourceItem(name, name));
    }
}
