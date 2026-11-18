using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class ProofreadHandler(InvocationContext invocationContext)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var sessions = await Client.Paginate<ProofreadDetails>(() => new RestRequest("video-translations/proofreads"),
            Picker.Limit, Picker.Limit,
            session => !string.IsNullOrWhiteSpace(session.ProofreadId)
                && Picker.Matches(context.SearchString, session.ProofreadId, session.Title, session.OutputLanguage),
            Picker.Pages);

        return sessions.DistinctBy(session => session.ProofreadId)
            .Select(session => Picker.Item(session.ProofreadId, session.Title,
                string.IsNullOrWhiteSpace(session.OutputLanguage)
                    ? session.Status
                    : $"{session.OutputLanguage} · {session.Status}"));
    }
}
