using Apps.HeyGen.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class InstantVoiceHandler(InvocationContext invocationContext)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var voices = await Client.Paginate<Voice>(
            () => new RestRequest("models/audio/voices").AddQueryParameter("type", "private"),
            40, Picker.Limit,
            voice => voice.Mode == "instant" && voice.Status == "ACTIVE" && !string.IsNullOrWhiteSpace(voice.Id)
                && Picker.Matches(context.SearchString, voice.Id, voice.Name, voice.Language),
            Picker.Pages);

        return voices.DistinctBy(voice => voice.Id)
            .Select(voice => Picker.Item(voice.Id, voice.Name, voice.Language));
    }
}
