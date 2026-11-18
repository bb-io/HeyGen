using Apps.HeyGen.Api;
using Apps.HeyGen.Files;
using Apps.HeyGen.Models.Inputs;
using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.HeyGen.Actions;

public abstract class SourceVideoActions(InvocationContext invocationContext, IFileManagementClient files)
    : Invocable(invocationContext)
{
    protected IFileManagementClient FileManagement { get; } = files;

    protected async Task<object> ResolveVideo(VideoSourceInput input, string key)
    {
        CheckSource(input.VideoFile, input.VideoUrl, input.VideoAssetId, "source video", input.VideoId);
        if (string.IsNullOrWhiteSpace(input.VideoId))
            return await ResolveSource(input.VideoFile, input.VideoUrl, input.VideoAssetId, key, "video");

        var id = Requests.Id(input.VideoId, "a video ID");
        var video = await Client.Data<VideoDetails>(new RestRequest($"videos/{id}"));
        if (video.Status != "completed" || string.IsNullOrWhiteSpace(video.VideoUrl))
            throw new PluginMisconfigurationException("Select a completed HeyGen video with a download URL.");

        return new { type = "url", url = MediaFiles.PublicUrl(video.VideoUrl) };
    }

    protected async Task<object> ResolveSource(FileReference? file, string? url, string? assetId, string key, string role)
    {
        if (file is not null)
        {
            var asset = await MediaFiles.Upload(Client, FileManagement, file, Requests.DeriveKey(key, role));
            return new { type = "asset_id", asset_id = asset.Id };
        }
        if (!string.IsNullOrWhiteSpace(assetId))
            return new { type = "asset_id", asset_id = assetId.Trim() };

        return new { type = "url", url = MediaFiles.PublicUrl(url!) };
    }

    protected static void CheckSource(FileReference? file, string? url, string? assetId, string field, string? videoId = null)
    {
        var count = new[] { file is not null, !string.IsNullOrWhiteSpace(url), !string.IsNullOrWhiteSpace(assetId),
            !string.IsNullOrWhiteSpace(videoId) }.Count(value => value);
        if (count != 1)
            throw new PluginMisconfigurationException($"Provide exactly one input for the {field}.");

        if (file is not null)
            MediaFiles.Validate(file, field == "source video" ? "video" : "audio");
        if (!string.IsNullOrWhiteSpace(url))
            _ = MediaFiles.PublicUrl(url);
    }
}
