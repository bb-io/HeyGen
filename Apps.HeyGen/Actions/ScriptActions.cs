using Apps.HeyGen.Api;
using Apps.HeyGen.Files;
using Apps.HeyGen.Models.Dtos;
using Apps.HeyGen.Models.Inputs;
using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.HeyGen.Actions;

[ActionList("Script localization")]
public class ScriptActions(InvocationContext invocationContext, IFileManagementClient files) : Invocable(invocationContext)
{
    [Action("Export video script as XLIFF", Description = "Export editable synthesized speech from a HeyGen video's scenes.")]
    public async Task<FileOutput> ExportVideoScript([ActionParameter] ScriptSelectionInput input)
    {
        var id = Requests.Id(input.VideoId, "a video ID");
        var scenes = await Client.Data<Scenes>(new RestRequest($"videos/{id}/scenes"));
        var content = scenes.Items.SelectMany(scene => scene.Script)
            .Where(entry => entry.Type == "text" && !string.IsNullOrWhiteSpace(entry.Text)
                && (string.IsNullOrWhiteSpace(input.ScriptEntryId) || entry.Id == input.ScriptEntryId.Trim()))
            .DistinctBy(entry => entry.Id)
            .ToDictionary(entry => entry.Id, entry => entry.Text!, StringComparer.Ordinal);

        await using var stream = XliffScript.Write($"heygen:video:{id}", input.SourceLanguage, input.TargetLanguage, content);
        return new FileOutput { File = await files.UploadAsync(stream, "application/x-xliff+xml", $"{id}-script.xlf") };
    }

    [Action("Read translated video script", Description = "Read translated speech from an exported video XLIFF for a new avatar render.")]
    public async Task<ScriptOutput> ReadVideoScript([ActionParameter] XliffInput input)
    {
        var bytes = await MediaFiles.ReadBytes(files, input.File, 2_000_000);
        var file = XliffScript.Read(bytes);
        const string prefix = "heygen:video:";
        if (!file.Original.StartsWith(prefix, StringComparison.Ordinal) || file.Original.Length == prefix.Length)
            throw new PluginMisconfigurationException("Select an XLIFF exported with Export video script as XLIFF.");

        var id = file.Original[prefix.Length..];
        var scenes = await Client.Data<Scenes>(new RestRequest($"videos/{Requests.Id(Uri.UnescapeDataString(id), "a source video ID")}/scenes"));
        var liveEntries = scenes.Items.SelectMany(scene => scene.Script)
            .Where(entry => entry.Type == "text" && !string.IsNullOrWhiteSpace(entry.Text))
            .DistinctBy(entry => entry.Id).ToList();
        var current = liveEntries.ToDictionary(entry => entry.Id, entry => entry.Text!, StringComparer.Ordinal);
        if (file.Units.Any(entry => !current.TryGetValue(entry.Key, out var source) || source != entry.Value.Source))
            throw new PluginMisconfigurationException("The source script changed or an XLIFF unit no longer matches. Export again and update the translation.");

        return new ScriptOutput
        {
            Script = string.Join("\n\n", liveEntries.Where(entry => file.Units.ContainsKey(entry.Id))
                .Select(entry => file.Units[entry.Id].Target)),
            TargetLanguage = file.TargetLanguage,
            VideoId = Uri.UnescapeDataString(id)
        };
    }
}
