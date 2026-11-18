using System.Net;
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

[ActionList("Asset batches")]
public class AssetBatchActions(InvocationContext invocationContext, IFileManagementClient files) : Invocable(invocationContext)
{
    private static readonly HttpClient Uploads = new(new HttpClientHandler { AllowAutoRedirect = false })
    {
        Timeout = TimeSpan.FromMinutes(15)
    };

    [Action("Create asset batch", Description = "Upload Blackbird files into a native HeyGen asset batch and start ingestion.")]
    public async Task<BatchSubmission> CreateBatch([ActionParameter] AssetBatchInput input)
    {
        var key = Requests.SubmissionKey(input.SubmissionKey);
        if (input.Files is null || input.Files.Count is < 1 or > 100)
            throw new PluginMisconfigurationException("Select 1–100 files for the HeyGen asset batch.");

        var manifest = new List<object>();
        var lengths = new List<long>();
        foreach (var file in input.Files)
        {
            var mime = MediaFiles.AssetBatchMime(file);
            var size = await MediaFiles.AssetBatchSize(files, file);
            lengths.Add(size);
            manifest.Add(new { filename = Path.GetFileName(file.Name), content_type = mime, size_bytes = size });
        }

        var request = Requests.Json("assets/direct-uploads/batches", Method.Post, new()
        {
            ["title"] = input.Title,
            ["files"] = manifest
        }).AddHeader("Idempotency-Key", key);
        var slots = await Client.Data<AssetBatchSlots>(request);

        if (slots.Items.Count != manifest.Count)
            throw new PluginApplicationException("HeyGen returned a different number of upload slots than requested. Inspect the batch before retrying.");

        try
        {
            for (var index = 0; index < slots.Items.Count; index++)
            {
                var slot = slots.Items[index];
                var file = input.Files[index];
                if (slot.MaximumBytes > 0 && lengths[index] > slot.MaximumBytes)
                    throw new PluginMisconfigurationException($"File {index + 1} exceeds the size returned by HeyGen.");

                if (!Uri.TryCreate(slot.UploadUrl, UriKind.Absolute, out var uri) || uri.Scheme != "https" ||
                    uri.IsLoopback || !string.IsNullOrEmpty(uri.UserInfo))
                    throw new PluginApplicationException("HeyGen returned an invalid direct-upload URL.");

                await using var stream = await files.DownloadAsync(file);
                using var content = new StreamContent(stream);
                content.Headers.ContentLength = lengths[index];
                using var upload = new HttpRequestMessage(HttpMethod.Put, uri) { Content = content };
                foreach (var (name, value) in slot.Headers)
                {
                    if (name.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (!content.Headers.TryAddWithoutValidation(name, value))
                        upload.Headers.TryAddWithoutValidation(name, value);
                }

                using var response = await Uploads.SendAsync(upload, HttpCompletionOption.ResponseHeadersRead,
                    InvocationContext.CancellationToken);
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new PluginApplicationException($"HeyGen asset upload {index + 1} was rejected by the storage service.");
            }
        }
        catch (Exception exception) when (exception is HttpRequestException or IOException or TaskCanceledException or PluginApplicationException)
        {
            throw new PluginApplicationException($"Asset batch {slots.BatchId} could not finish uploading. If its slots are still valid, retry with the same files and submission key.");
        }

        try
        {
            return await Complete(slots.BatchId);
        }
        catch (PluginApplicationException)
        {
            throw new PluginApplicationException($"Asset batch {slots.BatchId} was uploaded. Check its status or resume it with Complete asset batch.");
        }
    }

    [Action("Complete asset batch", Description = "Repeat ingestion for the same asset batch after all files have uploaded.")]
    public Task<BatchSubmission> CompleteBatch([ActionParameter] AssetBatchCompletionInput input) => Complete(input.BatchId);

    private async Task<BatchSubmission> Complete(string batchId)
    {
        var id = Requests.Required(batchId, "an asset batch ID").Trim();
        var request = Requests.Json("assets/complete/batches", Method.Post, new() { ["batch_id"] = id });
        var result = await Client.Data<BatchSubmission>(request);
        result.BatchType = "assets";
        return result;
    }
}
