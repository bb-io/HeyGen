using System.Text;
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

[ActionList("Audio")]
public class AudioActions(InvocationContext invocationContext, IFileManagementClient files)
    : SourceVideoActions(invocationContext, files)
{
    private const int SpeechTimeoutMilliseconds = 170_000;

    [Action("Text to speech", Description = "Turn written text into a narration file using a HeyGen voice.")]
    public async Task<SpeechOutput> TextToSpeech([ActionParameter] TextToSpeechInput input)
    {
        var text = Requests.Required(input.Text, "text to speak");
        if (text.EnumerateRunes().Count() > 10_000)
            throw new PluginMisconfigurationException("Use no more than 10,000 characters per HeyGen narration.");

        var voice = Requests.Required(input.VoiceId, "a voice ID").Trim();
        var format = string.IsNullOrWhiteSpace(input.Format) ? "wav" : input.Format.Trim().ToLowerInvariant();
        if (format is not ("wav" or "mp3"))
            throw new PluginMisconfigurationException("Choose WAV or MP3 for the narration file.");

        var language = string.IsNullOrWhiteSpace(input.Language) ? null : input.Language.Trim().ToLowerInvariant();
        if (language is not null && (language.Length != 2 || !language.All(char.IsAsciiLetterLower)))
            throw new PluginMisconfigurationException("Enter a base language code such as en or hu, or leave it blank.");

        var request = Requests.Json("models/audio/tts", Method.Post, new()
        {
            ["voice_id"] = voice,
            ["model"] = "heygen-voice-1",
            ["text"] = text,
            ["language"] = language,
            ["output_format"] = format == "mp3" ? new { encoding = "mp3" } : null
        });
        return await GenerateAudio(request, format, null);
    }

    [Action("Speech to speech", Description = "Revoice a recording with an instant voice while preserving its words and timing.")]
    public async Task<SpeechOutput> SpeechToSpeech([ActionParameter] SpeechToSpeechInput input)
    {
        var key = Requests.SubmissionKey(input.SubmissionKey);
        var voice = Requests.Required(input.VoiceId, "an instant voice ID").Trim();
        var (url, assetId) = await ResolveAudio(input, key, "speech-conversion");
        object audio = assetId is not null
            ? new { type = "asset_id", asset_id = assetId }
            : new { type = "url", url };
        var request = Requests.Json("models/audio/sts", Method.Post, new()
        {
            ["voice_id"] = voice,
            ["audio"] = audio,
            ["remove_background_noise"] = input.RemoveBackgroundNoise
        }).AddHeader("Idempotency-Key", key);
        return await GenerateAudio(request, "wav", key);
    }

    [Action("Create avatar video from audio", Description = "Submit an avatar render using recorded or approved narration.")]
    public async Task<VideoSubmission> CreateVideoFromAudio([ActionParameter] CreateVideoFromAudioInput input,
        [ActionParameter] AudioSourceInput audio)
    {
        var key = Requests.SubmissionKey(input.SubmissionKey);
        var avatar = Requests.Required(input.AvatarId, "an avatar ID").Trim();
        var (url, assetId) = await ResolveAudio(audio, key, "avatar-narration");

        return await Client.Data<VideoSubmission>(Requests.Json("videos", Method.Post, new()
        {
            ["type"] = "avatar",
            ["avatar_id"] = avatar,
            ["audio_url"] = url,
            ["audio_asset_id"] = assetId,
            ["engine"] = string.IsNullOrWhiteSpace(input.Engine) ? null : new { type = input.Engine },
            ["title"] = input.Title,
            ["aspect_ratio"] = input.AspectRatio,
            ["resolution"] = input.Resolution,
            ["caption"] = input.BurnCaptions == true ? new { style = "default" } : null,
            ["callback_id"] = input.CallbackId
        }).AddHeader("Idempotency-Key", key));
    }

    private async Task<(string? Url, string? AssetId)> ResolveAudio(AudioSourceInput input, string key, string purpose)
    {
        CheckSource(input.AudioFile, input.AudioUrl, input.AudioAssetId, "source audio");

        if (input.AudioFile is not null)
        {
            var asset = await MediaFiles.Upload(Client, FileManagement, input.AudioFile, Requests.DeriveKey(key, purpose));
            return (null, asset.Id);
        }

        if (!string.IsNullOrWhiteSpace(input.AudioAssetId))
            return (null, input.AudioAssetId.Trim());

        var url = MediaFiles.PublicUrl(input.AudioUrl!);
        if (!url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            throw new PluginMisconfigurationException("Enter a direct, publicly downloadable HTTPS audio URL.");

        return (url, null);
    }

    private async Task<SpeechOutput> GenerateAudio(RestRequest request, string format, string? submissionKey)
    {
        using var speechClient = new HeyGenClient(InvocationContext.AuthenticationCredentialsProviders, SpeechTimeoutMilliseconds);
        var response = await speechClient.ExecuteWithErrorHandling(request);
        var requestId = Header(response, "X-Request-Id");
        var audio = HeyGenClient.Read<Envelope<SpeechAudio>>(response).Data
            ?? throw new PluginApplicationException("HeyGen returned no audio data. Check usage before repeating this request.");

        try
        {
            var mime = format == "mp3" ? "audio/mpeg" : "audio/wav";
            var result = await MediaFiles.Download(FileManagement, audio.AudioUrl,
                $"heygen-narration-{Guid.NewGuid():N}.{format}", mime, InvocationContext.CancellationToken);
            return new SpeechOutput
            {
                File = result.File,
                Duration = audio.Duration,
                RequestId = requestId,
                Language = Header(response, "Content-Language")
            };
        }
        catch (Exception exception) when (exception is PluginApplicationException or PluginMisconfigurationException)
        {
            var retry = submissionKey is null
                ? " Check HeyGen usage before generating the narration again."
                : " If retrying within 24 hours, reuse the same submission key and inputs.";
            var reference = requestId is null ? "" : $" HeyGen request ID {requestId}.";
            throw new PluginApplicationException($"HeyGen accepted the speech request, but Blackbird could not retrieve the audio.{reference}{retry}");
        }
    }

    private static string? Header(RestResponse response, string name) =>
        response.Headers?.FirstOrDefault(header => string.Equals(header.Name, name, StringComparison.OrdinalIgnoreCase))
            ?.Value?.ToString();
}
