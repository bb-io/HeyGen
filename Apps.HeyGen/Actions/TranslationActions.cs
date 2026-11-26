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

[ActionList("Video translation")]
public class TranslationActions(InvocationContext invocationContext, IFileManagementClient files)
    : SourceVideoActions(invocationContext, files)
{
    [Action("Translate video", Description = "Start native HeyGen voice-cloned dubbing. Returns a translation ID for each requested language.")]
    public async Task<TranslationSubmission> TranslateVideo([ActionParameter] TranslateInput input)
    {
        var key = Requests.SubmissionKey(input.SubmissionKey);
        var languages = (input.Languages ?? []).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList();
        if (languages.Count == 0 || languages.Distinct(StringComparer.OrdinalIgnoreCase).Count() != languages.Count)
            throw new PluginMisconfigurationException("Select at least one target language and remove duplicates.");

        var request = Requests.Json("video-translations", Method.Post, new()
        {
            ["video"] = await ResolveVideo(input, key),
            ["output_languages"] = languages,
            ["mode"] = input.Mode,
            ["title"] = input.Title,
            ["translate_audio_only"] = input.AudioOnly,
            ["brand_glossary_id"] = input.BrandGlossaryId,
            ["callback_id"] = input.CallbackId
        }).AddHeader("Idempotency-Key", key);
        return await Client.Data<TranslationSubmission>(request);
    }

    [Action("Get video translation", Description = "Read a translation job's state, output language and artifact URLs.")]
    public Task<TranslationDetails> GetTranslation([ActionParameter] TranslationInput input) => ReadTranslation(input.TranslationId);

    [Action("Download translated video", Description = "Download the translated video's MP4 as a Blackbird file.")]
    public async Task<FileOutput> DownloadTranslation([ActionParameter] TranslationInput input)
    {
        var translation = await ReadTranslation(input.TranslationId);
        return await MediaFiles.Download(FileManagement, translation.VideoUrl, $"{Requests.Id(translation.TranslationId, "a translation ID")}.mp4",
            "video/mp4", InvocationContext.CancellationToken);
    }

    [Action("Get translation captions", Description = "Download the translated SRT or WebVTT as a Blackbird file.")]
    public async Task<FileOutput> GetTranslationCaptions([ActionParameter] TranslationCaptionInput input)
    {
        var format = string.IsNullOrWhiteSpace(input.Format) ? "srt" : input.Format;
        if (format is not ("srt" or "vtt"))
            throw new PluginMisconfigurationException("Choose SRT or WebVTT for translation captions.");

        var translation = await ReadTranslation(input.TranslationId);
        var id = Requests.Id(translation.TranslationId, "a translation ID");
        var url = format == "vtt" ? translation.VttUrl : translation.SrtUrl;
        var mime = format == "vtt" ? "text/vtt" : "application/x-subrip";
        return await MediaFiles.Download(FileManagement, url, $"{id}.{format}", mime, InvocationContext.CancellationToken);
    }

    private Task<TranslationDetails> ReadTranslation(string id) =>
        Client.Data<TranslationDetails>(new RestRequest($"video-translations/{Requests.Id(id, "a translation ID")}"));
}
