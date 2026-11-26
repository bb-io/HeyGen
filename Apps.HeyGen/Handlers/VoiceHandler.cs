using Apps.HeyGen.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class VoiceHandler(InvocationContext invocationContext)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var privateVoices = await Client.Paginate<Voice>(
            () => CreateRequest("private", null), 20, 20,
            x => x.Status == "ACTIVE" && Picker.Matches(context.SearchString, x.Id, x.Name, x.Language), Picker.Pages);
        var publicVoices = await Client.Paginate<Voice>(
            () => CreateRequest("public", context.SearchString), Picker.Limit, Picker.Limit,
            x => !string.IsNullOrWhiteSpace(x.Id), Picker.Pages);
        return privateVoices.Select(x => (Voice: x, Scope: "Workspace"))
            .Concat(publicVoices.Select(x => (Voice: x, Scope: "Public")))
            .DistinctBy(x => x.Voice.Id)
            .Select(x => Picker.Item(x.Voice.Id, x.Voice.Name, $"{x.Scope} · {x.Voice.Language ?? "language unavailable"}"));
    }

    private static RestRequest CreateRequest(string type, string? query)
    {
        var request = new RestRequest("models/audio/voices").AddQueryParameter("type", type);
        if (!string.IsNullOrWhiteSpace(query))
            request.AddQueryParameter("query", query.Trim());
        return request;
    }
}
