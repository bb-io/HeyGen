using Apps.HeyGen.Api;
using Apps.HeyGen.Files;
using Apps.HeyGen.Models.Inputs;
using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.HeyGen.Actions;

[ActionList("Lipsync")]
public class LipsyncActions(InvocationContext invocationContext, IFileManagementClient files)
    : SourceVideoActions(invocationContext, files)
{
    [Action("Create lipsync", Description = "Replace the audio on a video and submit it for lip re-animation.")]
    public async Task<LipsyncSubmission> CreateLipsync([ActionParameter] LipsyncInput input)
    {
        var key = Requests.SubmissionKey(input.SubmissionKey);
        CheckSource(input.AudioFile, input.AudioUrl, input.AudioAssetId, "replacement audio");

        var video = await ResolveVideo(input, key);
        var audio = await ResolveSource(input.AudioFile, input.AudioUrl, input.AudioAssetId, key, "audio");
        return await Client.Data<LipsyncSubmission>(Requests.Json("lipsyncs", Method.Post, new()
        {
            ["video"] = video,
            ["audio"] = audio,
            ["mode"] = input.Mode,
            ["title"] = input.Title,
            ["callback_id"] = input.CallbackId
        }).AddHeader("Idempotency-Key", key));
    }

    [Action("Get lipsync", Description = "Read a lipsync job's status and artifact URLs.")]
    public Task<LipsyncDetails> GetLipsync([ActionParameter] LipsyncJobInput input) => ReadLipsync(input.LipsyncId);

    [Action("Download lipsync video", Description = "Download the completed lipsync render as a Blackbird file.")]
    public async Task<FileOutput> DownloadLipsync([ActionParameter] LipsyncJobInput input)
    {
        var job = await ReadLipsync(input.LipsyncId);
        return await MediaFiles.Download(FileManagement, job.VideoUrl, $"{Requests.Id(job.LipsyncId, "a lipsync ID")}.mp4",
            "video/mp4", InvocationContext.CancellationToken);
    }

    private Task<LipsyncDetails> ReadLipsync(string id) =>
        Client.Data<LipsyncDetails>(new RestRequest($"lipsyncs/{Requests.Id(id, "a lipsync ID")}"));
}
