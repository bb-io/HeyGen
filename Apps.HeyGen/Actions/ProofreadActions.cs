using System.Security.Cryptography;
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

[ActionList("Translation proofreading")]
public class ProofreadActions(InvocationContext invocationContext, IFileManagementClient files)
    : SourceVideoActions(invocationContext, files)
{
    private const string SessionsPath = "video-translations/proofreads";
    private const string SubRip = "application/x-subrip";

    [Action("Start translation proofread", Description = "Prepare an editable translated SRT and return a proofread session ID for each language.")]
    public async Task<ProofreadSubmission> StartProofread([ActionParameter] StartProofreadInput input)
    {
        var key = Requests.SubmissionKey(input.SubmissionKey);
        var title = Requests.Required(input.Title, "a review title").Trim();
        var languages = (input.Languages ?? []).Where(language => !string.IsNullOrWhiteSpace(language))
            .Select(language => language.Trim()).ToList();
        if (languages.Count == 0 || languages.Distinct(StringComparer.OrdinalIgnoreCase).Count() != languages.Count)
            throw new PluginMisconfigurationException("Select at least one target language and remove duplicates.");

        var mode = string.IsNullOrWhiteSpace(input.Mode) ? "speed" : input.Mode.Trim();
        if (mode is not ("speed" or "precision"))
            throw new PluginMisconfigurationException("Choose Speed or Precision for the proofread session.");

        var request = Requests.Json(SessionsPath, Method.Post, new()
        {
            ["video"] = await ResolveVideo(input, key),
            ["output_languages"] = languages,
            ["title"] = title,
            ["mode"] = mode,
            ["brand_glossary_id"] = input.BrandGlossaryId
        }).AddHeader("Idempotency-Key", key);

        return await Client.Data<ProofreadSubmission>(request);
    }

    [Action("Get proofread session", Description = "Check when translated subtitles are ready and read the language and failure details.")]
    public Task<ProofreadDetails> GetProofread([ActionParameter] ProofreadInput input) =>
        Client.Data<ProofreadDetails>(new RestRequest(SessionPath(input.ProofreadId)));

    [Action("Download proofread SRT", Description = "Download the current translated SRT from a completed proofread session.")]
    public async Task<FileOutput> DownloadProofreadSrt([ActionParameter] ProofreadInput input)
    {
        var id = Requests.Id(input.ProofreadId, "a proofread session ID");
        var download = await Client.Data<ProofreadSrt>(new RestRequest($"{SessionsPath}/{id}/srt"));
        return await MediaFiles.Download(FileManagement, download.Url, $"{id}-proofread.srt",
            SubRip, InvocationContext.CancellationToken);
    }

    [Action("Upload edited proofread SRT", Description = "Replace the session's translated subtitles with the linguist-approved SRT.")]
    public async Task<ProofreadDetails> UploadProofreadSrt([ActionParameter] UploadProofreadInput input)
    {
        var path = SessionPath(input.ProofreadId);
        var count = new[] { input.File is not null, !string.IsNullOrWhiteSpace(input.Url),
            !string.IsNullOrWhiteSpace(input.AssetId) }.Count(present => present);
        if (count != 1)
            throw new PluginMisconfigurationException("Provide exactly one edited SRT source: a Blackbird file, HTTPS URL, or HeyGen asset ID.");

        var srt = await ResolveSrt(input, path);
        var request = Requests.Json($"{path}/srt", Method.Put, new() { ["srt"] = srt });
        return await Client.Data<ProofreadDetails>(request);
    }

    [Action("Generate video from proofread", Description = "Render the dubbed source footage using the current proofread subtitles and return a translation ID.")]
    public Task<ProofreadRender> RenderProofread([ActionParameter] RenderProofreadInput input)
    {
        var key = Requests.SubmissionKey(input.SubmissionKey);
        var path = SessionPath(input.ProofreadId);
        return Client.Data<ProofreadRender>(Requests.Json($"{path}/generate", Method.Post, new())
            .AddHeader("Idempotency-Key", key));
    }

    private async Task<object> ResolveSrt(UploadProofreadInput input, string path)
    {
        if (input.File is not null)
        {
            if (!".srt".Equals(Path.GetExtension(input.File.Name), StringComparison.OrdinalIgnoreCase))
                throw new PluginMisconfigurationException("Choose a subtitle file with the .srt extension.");

            var session = await Client.Data<ProofreadDetails>(new RestRequest(path));
            if (session.Status != "completed")
                throw new PluginMisconfigurationException("Upload edited subtitles once Get proofread session reports Completed.");

            var bytes = await MediaFiles.ReadBytes(FileManagement, input.File, 50_000_000);
            var hash = Convert.ToHexString(SHA256.HashData(bytes));
            var assetKey = Requests.DeriveKey($"{path}:{hash}", "proofread-srt");
            var upload = new RestRequest("assets", Method.Post).AddFile("file", bytes, "proofread.srt", SubRip)
                .AddHeader("Idempotency-Key", assetKey);

            var asset = await Client.Data<Asset>(upload);
            return new { type = "asset_id", asset_id = asset.Id };
        }

        if (!string.IsNullOrWhiteSpace(input.AssetId))
            return new { type = "asset_id", asset_id = input.AssetId.Trim() };

        var url = MediaFiles.PublicUrl(input.Url!);
        if (!url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            throw new PluginMisconfigurationException("Enter a direct HTTPS URL for the edited SRT.");
        return new { type = "url", url };
    }

    private static string SessionPath(string id) => $"{SessionsPath}/{Requests.Id(id, "a proofread session ID")}";
}
