using Apps.HeyGen.Api;
using Apps.HeyGen.Files;
using Apps.HeyGen.Models.Inputs;
using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.HeyGen.Actions;

[ActionList("AI Clipping")]
public class ClippingActions(InvocationContext invocationContext, IFileManagementClient files)
    : SourceVideoActions(invocationContext, files)
{
    [Action("Create AI clipping job", Description = "Generate short highlights from a source video.")]
    public async Task<ClippingSubmission> CreateClipping([ActionParameter] ClippingInput input)
    {
        var key = Requests.SubmissionKey(input.SubmissionKey);
        if (input.Prompt?.Length > 500)
            throw new PluginMisconfigurationException("Use at most 500 characters for editorial guidance.");

        var settings = new Dictionary<string, object?>();
        if (input.Durations?.Any() == true)
            settings["duration_types"] = input.Durations.Distinct().ToArray();
        if (input.AspectRatio is not null)
            settings["aspect_ratio"] = input.AspectRatio;
        if (input.Captions is not null)
            settings["captions"] = input.Captions;
        if (!string.IsNullOrWhiteSpace(input.Prompt))
            settings["prompt"] = input.Prompt;

        return await Client.Data<ClippingSubmission>(Requests.Json("ai-clipping", Method.Post, new()
        {
            ["video"] = await ResolveVideo(input, key),
            ["title"] = input.Title,
            ["input_language"] = input.InputLanguage,
            ["output_settings"] = settings.Count == 0 ? null : settings,
            ["callback_id"] = input.CallbackId
        }).AddHeader("Idempotency-Key", key));
    }

    [Action("Get AI clipping job", Description = "Read progress and produced clips with their virality scores and artifact URLs.")]
    public Task<ClippingDetails> GetClipping([ActionParameter] ClippingJobInput input) => ReadClipping(input.ClippingJobId);

    [Action("Download clip", Description = "Download a completed highlight clip from a clipping job.")]
    public async Task<FileOutput> DownloadClip([ActionParameter] DownloadClipInput input)
    {
        var job = await ReadClipping(input.ClippingJobId);
        var clip = job.Clips.FirstOrDefault(x => x.ClipId == input.ClipId?.Trim());
        if (clip is null)
            throw new PluginMisconfigurationException("Choose a clip ID that belongs to the selected clipping job.");

        return await MediaFiles.Download(FileManagement, clip.VideoUrl, $"{Requests.Id(clip.ClipId, "a clip ID")}.mp4",
            "video/mp4", InvocationContext.CancellationToken);
    }

    private Task<ClippingDetails> ReadClipping(string id) =>
        Client.Data<ClippingDetails>(new RestRequest($"ai-clipping/{Requests.Id(id, "a clipping job ID")}"));
}
