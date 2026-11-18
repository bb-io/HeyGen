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

[ActionList("Videos")]
public class VideoActions(InvocationContext invocationContext, IFileManagementClient files) : Invocable(invocationContext)
{
    [Action("Search videos", Description = "Find accessible HeyGen videos by title and status.")]
    public async Task<VideoResults> SearchVideos([ActionParameter] VideoSearchInput input)
    {
        var maximum = input.Maximum ?? 100;
        if (maximum is < 1 or > 1000)
            throw new PluginMisconfigurationException("Enter a maximum result count between 1 and 1,000.");

        var videos = await Client.Paginate<VideoDetails>(() => SearchRequest(input.Title), 100, maximum,
            video => string.IsNullOrWhiteSpace(input.Status) || video.Status == input.Status);
        return new VideoResults { Videos = videos };
    }

    [Action("Get video", Description = "Read the current status, metadata and download URLs for a video.")]
    public Task<VideoDetails> GetVideo([ActionParameter] VideoInput input) => ReadVideo(input.VideoId);

    [Action("Download video", Description = "Download the rendered video as a Blackbird file.")]
    public async Task<FileOutput> DownloadVideo([ActionParameter] VideoInput input)
    {
        var video = await ReadVideo(input.VideoId);
        return await MediaFiles.Download(files, video.VideoUrl, $"{Requests.Id(video.VideoId, "a video ID")}.mp4",
            "video/mp4", InvocationContext.CancellationToken);
    }

    [Action("Get captions", Description = "Download the SRT sidecar for a rendered video as a Blackbird file.")]
    public async Task<FileOutput> GetCaptions([ActionParameter] VideoInput input)
    {
        var video = await ReadVideo(input.VideoId);
        return await MediaFiles.Download(files, video.SubtitleUrl, $"{Requests.Id(video.VideoId, "a video ID")}.srt",
            "application/x-subrip", InvocationContext.CancellationToken);
    }

    [Action("Create video", Description = "Submit a single-scene avatar video from a script and return its video ID.")]
    public Task<VideoSubmission> CreateVideo([ActionParameter] CreateVideoInput input)
    {
        var key = Requests.SubmissionKey(input.SubmissionKey);
        var script = Requests.Required(input.Script, "script text");
        if (script.Length > 5000)
            throw new PluginMisconfigurationException("Use a script of at most 5,000 characters per avatar video.");

        return Client.Data<VideoSubmission>(Requests.Json("videos", Method.Post, new()
        {
            ["type"] = "avatar",
            ["avatar_id"] = Requests.Required(input.AvatarId, "an avatar ID").Trim(),
            ["script"] = script,
            ["voice_id"] = input.VoiceId,
            ["engine"] = string.IsNullOrWhiteSpace(input.Engine) ? null : new { type = input.Engine },
            ["title"] = input.Title,
            ["aspect_ratio"] = input.AspectRatio,
            ["resolution"] = input.Resolution,
            ["caption"] = input.BurnCaptions == true ? new { style = "default" } : null,
            ["brand_glossary_id"] = input.BrandGlossaryId,
            ["callback_id"] = input.CallbackId
        }).AddHeader("Idempotency-Key", key));
    }

    private Task<VideoDetails> ReadVideo(string id) =>
        Client.Data<VideoDetails>(new RestRequest($"videos/{Requests.Id(id, "a video ID")}"));

    private static RestRequest SearchRequest(string? title)
    {
        var request = new RestRequest("videos");
        if (!string.IsNullOrWhiteSpace(title))
            request.AddQueryParameter("title", title.Trim());
        return request;
    }
}
