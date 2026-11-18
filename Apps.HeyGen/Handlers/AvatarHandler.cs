using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Models.Dtos;
using Apps.HeyGen.Models.Inputs;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class AvatarHandler(InvocationContext invocationContext, [ActionParameter] AvatarGroupInput input)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var items = await Client.Paginate<AvatarLook>(CreateRequest, Picker.Limit, Picker.Limit,
            x => !string.IsNullOrWhiteSpace(x.Id) && x.Status is not ("processing" or "failed")
                && Picker.Matches(context.SearchString, x.Id, x.Name, x.Type), Picker.Pages);

        return items.DistinctBy(x => x.Id)
            .Select(x => Picker.Item(x.Id, x.Name, x.Type?.Replace('_', ' ')));
    }

    private RestRequest CreateRequest()
    {
        var request = new RestRequest("avatars/looks");
        if (!string.IsNullOrWhiteSpace(input?.AvatarGroupId))
            request.AddQueryParameter("group_id", input.AvatarGroupId.Trim());
        return request;
    }
}
