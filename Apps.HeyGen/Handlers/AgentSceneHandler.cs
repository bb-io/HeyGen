using Apps.HeyGen.Api;
using Apps.HeyGen.Models.Dtos;
using Apps.HeyGen.Models.Inputs;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class AgentSceneHandler(InvocationContext invocationContext, [ActionParameter] AgentDraftInput input)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input?.DraftVideoId))
            return [];

        var id = Requests.Id(input.DraftVideoId, "a draft video ID");
        var scenes = await Client.Data<Scenes>(new RestRequest($"videos/{id}/scenes"));
        return scenes.Items.Select((scene, index) => (Scene: scene, Number: index + 1, Text: scene.Narration()))
            .Where(item => !string.IsNullOrWhiteSpace(item.Scene.Id) &&
                Requests.Matches(context.SearchString, item.Scene.Id, item.Text, $"Scene {item.Number}"))
            .Select(item => new DataSourceItem(item.Scene.Id, FormatLabel(item.Number, item.Text)));
    }

    private static string FormatLabel(int number, string script)
    {
        var preview = string.Join(" ", script.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        return preview.Length switch
        {
            0 => $"Scene {number} · No text narration",
            > 90 => $"Scene {number} · {preview[..90]}…",
            _ => $"Scene {number} · {preview}"
        };
    }
}
