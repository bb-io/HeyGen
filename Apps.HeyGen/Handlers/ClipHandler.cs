using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Api;
using Apps.HeyGen.Models.Inputs;
using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class ClipHandler(InvocationContext invocationContext, [ActionParameter] ClippingJobInput input)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input?.ClippingJobId))
            return [];

        var id = Requests.Id(input.ClippingJobId, "a clipping job ID");
        var job = await Client.Data<ClippingDetails>(new RestRequest($"ai-clipping/{id}"));
        return job.Clips.Where(x => x.Status == "completed" && Picker.Matches(context.SearchString, x.ClipId, x.Title))
            .Select(x => Picker.Item(x.ClipId, x.Title, x.Duration is double duration ? $"{duration:0.#} s" : null));
    }
}
