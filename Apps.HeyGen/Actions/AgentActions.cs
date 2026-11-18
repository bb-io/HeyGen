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

[ActionList("Video Agent")]
public class AgentActions(InvocationContext invocationContext, IFileManagementClient files) : Invocable(invocationContext)
{
    [Action("Create Video Agent session", Description = "Generate a private Video Agent video from a prompt and optional reference files.")]
    public async Task<AgentDetails> CreateSession([ActionParameter] AgentAvatarInput input)
    {
        var prompt = Requests.Required(input.Prompt, "a prompt");
        if (prompt.Length > 10000)
            throw new PluginMisconfigurationException("Use a Video Agent prompt of at most 10,000 characters.");

        var attachments = input.Files ?? [];
        if (attachments.Count > 20)
            throw new PluginMisconfigurationException("Use at most 20 reference files.");
        foreach (var attachment in attachments)
            MediaFiles.Validate(attachment, null);

        var assets = new List<object>();
        foreach (var attachment in attachments)
        {
            var asset = await MediaFiles.Upload(Client, files, attachment);
            assets.Add(new { type = "asset_id", asset_id = asset.Id });
        }

        return await Client.Data<AgentDetails>(Requests.Json("video-agents", Method.Post, new()
        {
            ["mode"] = "generate",
            ["visibility"] = "private",
            ["prompt"] = prompt,
            ["avatar_id"] = input.AvatarId,
            ["voice_id"] = input.VoiceId,
            ["style_id"] = input.AgentStyleId,
            ["brand_glossary_id"] = input.BrandGlossaryId,
            ["orientation"] = input.Orientation,
            ["files"] = assets.Count == 0 ? null : assets,
            ["callback_id"] = input.CallbackId
        }));
    }

    [Action("Get Video Agent session", Description = "Read a session's status and the video ID when HeyGen has assigned it.")]
    public Task<AgentDetails> GetSession([ActionParameter] AgentSessionInput input) =>
        Client.Data<AgentDetails>(new RestRequest($"video-agents/{Requests.Id(input.AgentSessionId, "a session ID")}"));

    [Action("List Video Agent drafts", Description = "List the videos produced by a Video Agent session, newest first.")]
    public Task<VideoResults> ListDrafts([ActionParameter] AgentSessionInput input) =>
        AgentDrafts.Read(Client, input.AgentSessionId);

    [Action("List video scenes", Description = "Read the reviewed draft's scene IDs, narration, and edit version.")]
    public async Task<VideoSceneResults> ListScenes([ActionParameter] VideoInput input)
    {
        var id = Requests.Id(input.VideoId, "a draft video ID");
        var result = await Client.Data<Scenes>(new RestRequest($"videos/{id}/scenes"));
        if (string.IsNullOrWhiteSpace(result.EditVersion))
            throw new PluginApplicationException("HeyGen did not return an edit version. Check that the draft has finished preparing.");

        return new VideoSceneResults
        {
            DraftVideoId = input.VideoId.Trim(),
            EditVersion = result.EditVersion,
            Scenes = result.Items.Select((scene, index) => new VideoSceneItem
            {
                SceneId = scene.Id,
                Number = index + 1,
                Script = scene.Narration()
            }).ToList()
        };
    }

    [Action("Revise selected scenes", Description = "Submit approved scene instructions and return the new working draft's video ID when available.")]
    public Task<AgentRevisionResult> ReviseScenes([ActionParameter] ReviseAgentScenesInput input)
    {
        var session = Requests.Id(input.AgentSessionId, "an Agent session ID");
        var draft = Requests.Required(input.DraftVideoId, "the reviewed draft video ID").Trim();
        var version = Requests.Required(input.EditVersion, "the edit version returned by List video scenes").Trim();
        if (version.Length != 64 || version.Any(character => character is not (>= '0' and <= '9' or >= 'a' and <= 'f')))
            throw new PluginMisconfigurationException("Use the edit version returned by List video scenes for this draft.");

        var key = Requests.SubmissionKey(input.SubmissionKey);
        var edits = input.Edits ?? [];
        if (edits.Count > 0 && (!string.IsNullOrWhiteSpace(input.SceneId) || !string.IsNullOrWhiteSpace(input.Instruction)))
            throw new PluginMisconfigurationException("Provide a single scene and instruction or use the scene edits list.");

        if (edits.Count == 0)
            edits = [new() { SceneId = input.SceneId ?? "", Instruction = input.Instruction ?? "" }];
        if (edits.Count > 50)
            throw new PluginMisconfigurationException("Provide at most 50 scene edits in one revision.");

        var seen = new HashSet<string>(StringComparer.Ordinal);
        var plan = new List<object>();
        foreach (var edit in edits)
        {
            var sceneId = Requests.Required(edit?.SceneId, "a scene ID from the reviewed draft").Trim();
            var instruction = Requests.Required(edit?.Instruction, "an instruction for each selected scene");
            if (sceneId.EnumerateRunes().Count() > 255 || !seen.Add(sceneId))
                throw new PluginMisconfigurationException("Use a unique valid scene ID once in the revision.");
            if (instruction.EnumerateRunes().Count() > 10_000)
                throw new PluginMisconfigurationException("Limit each scene instruction to 10,000 characters.");

            plan.Add(new { scene_id = sceneId, text = instruction, scene_snapshot_video_id = draft, edit_version = version });
        }

        return Client.Data<AgentRevisionResult>(Requests.Json($"video-agents/{session}", Method.Post, new()
        {
            ["edit_plan"] = plan
        }).AddHeader("Idempotency-Key", key));
    }
}
