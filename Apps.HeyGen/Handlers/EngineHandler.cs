using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Api;
using Apps.HeyGen.Models.Dtos;
using Apps.HeyGen.Models.Inputs;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class EngineHandler(InvocationContext invocationContext, [ActionParameter] AvatarInput input)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    private static readonly Dictionary<string, string> Labels = new()
    {
        ["avatar_iii"] = "Avatar III",
        ["avatar_iv"] = "Avatar IV",
        ["avatar_v"] = "Avatar V"
    };

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input?.AvatarId))
            return [];

        var id = Requests.Id(input.AvatarId, "an avatar ID");
        var look = await Client.Data<AvatarLook>(new RestRequest($"avatars/looks/{id}"));
        return look.SupportedEngines.Distinct()
            .Where(x => Labels.ContainsKey(x) && Requests.Matches(context.SearchString, x, Labels[x]))
            .Select(x => new DataSourceItem(x, Labels[x]));
    }
}
