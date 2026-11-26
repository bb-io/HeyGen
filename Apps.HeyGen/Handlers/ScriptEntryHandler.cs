using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Api;
using Apps.HeyGen.Models.Dtos;
using Apps.HeyGen.Models.Inputs;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class ScriptEntryHandler(InvocationContext invocationContext, [ActionParameter] VideoInput input)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input?.VideoId))
            return [];

        var id = Requests.Id(input.VideoId, "a video ID");
        var scenes = await Client.Data<Scenes>(new RestRequest($"videos/{id}/scenes"));
        return scenes.Items.SelectMany((scene, index) => scene.Script.Select(entry => (Entry: entry, Scene: index + 1)))
            .Where(x => x.Entry.Type == "text" && !string.IsNullOrWhiteSpace(x.Entry.Text)
                && Requests.Matches(context.SearchString, x.Entry.Id, x.Entry.Text))
            .DistinctBy(x => x.Entry.Id)
            .Select(x => Picker.Item(x.Entry.Id, x.Entry.Text!.Length > 90 ? x.Entry.Text[..90] + "…" : x.Entry.Text,
                $"Scene {x.Scene}"));
    }
}
