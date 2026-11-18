using System.Text;
using Apps.HeyGen.Api;
using Apps.HeyGen.Files;
using Apps.HeyGen.Models.Inputs;
using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.HeyGen.Actions;

[ActionList("Studio video")]
public class StudioVideoActions(InvocationContext invocationContext, IFileManagementClient files)
    : Invocable(invocationContext)
{
    [Action("Create Studio video", Description = "Compose an ordered list of avatar, image and video scenes into one HeyGen render.")]
    public async Task<VideoSubmission> CreateStudioVideo([ActionParameter] CreateStudioVideoInput input)
    {
        var key = ResolveSubmissionKey(input);
        var inputs = ReadScenes(input);
        if (inputs.Count is < 1 or > 50)
            throw new PluginMisconfigurationException("Provide between 1 and 50 Studio scenes.");

        for (var index = 0; index < inputs.Count; index++)
            ValidateScene(inputs[index], index + 1);

        var scenes = new List<Dictionary<string, object?>>();
        for (var index = 0; index < inputs.Count; index++)
            scenes.Add(await BuildScene(inputs[index], key, index + 1));

        var request = Requests.Json("videos", Method.Post, new()
        {
            ["type"] = "studio",
            ["scenes"] = scenes,
            ["title"] = Normalize(input.Title),
            ["aspect_ratio"] = Normalize(input.AspectRatio),
            ["resolution"] = Normalize(input.Resolution),
            ["caption"] = input.BurnCaptions == true ? new { style = "default" } : null,
            ["brand_glossary_id"] = Normalize(input.BrandGlossaryId)
        }).AddHeader("Idempotency-Key", key);

        return await Client.Data<VideoSubmission>(request);
    }

    private string ResolveSubmissionKey(CreateStudioVideoInput input)
    {
        var mode = Normalize(input.SubmissionKeyMode) ?? "custom";
        if (mode == "custom")
            return Requests.SubmissionKey(input.SubmissionKey);

        if (mode != "automatic")
            throw new PluginMisconfigurationException("Choose Automatic per Flight or Custom key.");
        if (!string.IsNullOrWhiteSpace(input.SubmissionKey))
            throw new PluginMisconfigurationException("Clear the custom submission key when using Automatic per Flight.");

        var flightId = InvocationContext.Flight?.Id;
        if (string.IsNullOrWhiteSpace(flightId))
            throw new PluginMisconfigurationException("Automatic requires a Blackbird Flight ID. Choose Custom key in this context.");

        var identity = JsonConvert.SerializeObject(new[] { flightId, Normalize(input.RenderSlot) ?? "studio" });
        return Requests.DeriveKey(identity, "studio-render");
    }

    private static List<StudioSceneInput> ReadScenes(CreateStudioVideoInput input)
    {
        if (string.IsNullOrWhiteSpace(input.ScenesJson))
            return input.Scenes ?? [];
        if (input.Scenes is { Count: > 0 })
            throw new PluginMisconfigurationException("Provide either Scenes or Scenes JSON.");

        var array = Requests.JsonInput<JArray>(input.ScenesJson, "Studio scenes");
        if (array.Count is < 1 or > 50 || array.Any(item => item is not JObject))
            throw new PluginMisconfigurationException("Provide a JSON array containing 1 to 50 scene objects.");

        try
        {
            var serializer = JsonSerializer.Create(new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error });
            var scenes = array.ToObject<List<StudioSceneInput>>(serializer) ?? [];
            if (scenes.Any(scene => scene.MediaFile is not null || scene.AudioFile is not null))
                throw new PluginMisconfigurationException("Use structured Scenes to map Blackbird files, or use URLs or asset IDs in Scenes JSON.");

            return scenes;
        }
        catch (JsonException)
        {
            throw new PluginMisconfigurationException("Use the documented Studio scene fields and value types in Scenes JSON.");
        }
    }

    private static void ValidateScene(StudioSceneInput? scene, int number)
    {
        if (scene is null || scene.Type is not ("avatar_video" or "image" or "video"))
            throw Invalid(number, "Choose a speaking avatar, still image, or video clip.");

        var script = !string.IsNullOrWhiteSpace(scene.Script);
        var audio = SourceCount(scene.AudioFile, scene.AudioUrl, scene.AudioAssetId);
        var visual = SourceCount(scene.MediaFile, scene.MediaUrl, scene.MediaAssetId);

        if (audio + (script ? 1 : 0) > 1)
            throw Invalid(number, "Provide script or one narration source.");
        if (script && scene.Script!.EnumerateRunes().Count() > 5_000)
            throw Invalid(number, "Use at most 5,000 characters for the script.");
        if (!script && !string.IsNullOrWhiteSpace(scene.VoiceId))
            throw Invalid(number, "Choose a voice ID only for a script.");
        if (script && scene.Type != "avatar_video" && string.IsNullOrWhiteSpace(scene.VoiceId))
            throw Invalid(number, "Choose a voice ID for a scripted image or clip.");
        if (scene.Type != "image" && scene.Duration is not null)
            throw Invalid(number, "Set an explicit duration only for a silent still image.");
        if (scene.Type != "video" && scene.MuteClip is not null)
            throw Invalid(number, "Set clip audio muting only on video clips.");

        if (scene.Type == "avatar_video")
        {
            if (string.IsNullOrWhiteSpace(scene.AvatarId) || visual != 0 || (!script && audio == 0))
                throw Invalid(number, "Provide an avatar look ID and a script or narration source. Leave media inputs empty.");
        }
        else if (visual != 1 || !string.IsNullOrWhiteSpace(scene.AvatarId))
        {
            throw Invalid(number, "Provide exactly one visual source and leave Avatar look ID empty.");
        }

        if (scene.Type == "image")
        {
            var narrated = script || audio > 0;
            if (narrated && scene.Duration is not null || !narrated &&
                (scene.Duration is not double duration || !double.IsFinite(duration) || duration is <= 0 or > 300))
                throw Invalid(number, "Use narration or set a silent image duration above 0 and up to 300 seconds.");
        }

        ValidateSource(scene.AudioFile, scene.AudioUrl, "audio", number);
        ValidateSource(scene.MediaFile, scene.MediaUrl, scene.Type, number);
    }

    private async Task<Dictionary<string, object?>> BuildScene(StudioSceneInput scene, string key, int number)
    {
        var source = await ResolveSource(scene.MediaFile, scene.MediaUrl, scene.MediaAssetId, key, $"studio-{number}-visual");
        var audio = await ResolveSource(scene.AudioFile, scene.AudioUrl, scene.AudioAssetId, key, $"studio-{number}-audio");
        var body = new Dictionary<string, object?>();
        Add(body, "script", string.IsNullOrWhiteSpace(scene.Script) ? null : scene.Script);
        Add(body, "voice_id", Normalize(scene.VoiceId));
        Add(body, "audio_url", audio.Url);
        Add(body, "audio_asset_id", audio.AssetId);

        if (scene.Type == "avatar_video")
        {
            body["type"] = "avatar";
            body["avatar_id"] = Normalize(scene.AvatarId);
            return new() { ["type"] = scene.Type, ["input"] = body };
        }

        body["type"] = scene.Type;
        body["source"] = source.AssetId is not null
            ? new { type = "asset_id", asset_id = source.AssetId }
            : new { type = "url", url = source.Url };
        Add(body, "duration", scene.Duration);
        Add(body, "playback", scene.MuteClip is bool mute ? new { mute } : null);
        return body;
    }

    private async Task<(string? Url, string? AssetId)> ResolveSource(FileReference? file, string? url,
        string? assetId, string key, string purpose)
    {
        if (file is null)
            return (string.IsNullOrWhiteSpace(url) ? null : MediaFiles.PublicUrl(url), Normalize(assetId));

        var upload = await MediaFiles.Upload(Client, files, file, Requests.DeriveKey(key, purpose));
        return (null, upload.Id);
    }

    private static void ValidateSource(FileReference? file, string? url, string kind, int number)
    {
        try
        {
            if (file is not null)
                MediaFiles.Validate(file, kind);
            if (!string.IsNullOrWhiteSpace(url))
                _ = MediaFiles.PublicUrl(url);
        }
        catch (PluginMisconfigurationException exception)
        {
            throw Invalid(number, exception.Message);
        }
    }

    private static int SourceCount(FileReference? file, string? url, string? id) =>
        (file is null ? 0 : 1) + (string.IsNullOrWhiteSpace(url) ? 0 : 1) + (string.IsNullOrWhiteSpace(id) ? 0 : 1);

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static void Add(Dictionary<string, object?> target, string name, object? value)
    {
        if (value is not null)
            target.Add(name, value);
    }

    private static PluginMisconfigurationException Invalid(int number, string message) =>
        new($"Studio scene {number}. {message}");
}
