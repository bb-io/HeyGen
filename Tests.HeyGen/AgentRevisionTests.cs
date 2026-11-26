using Apps.HeyGen.Actions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.HeyGen.Base;

namespace Tests.HeyGen;

[TestClass]
public class AgentRevisionTests : TestBase
{
    [TestMethod]
    public async Task ListDraftsIncludesKnownVideo()
    {
        var result = await new AgentActions(Context, Files).ListDrafts(new() { AgentSessionId = Fixture("AgentSessionId") });

        Assert.IsTrue(result.Videos.Any(video => video.VideoId == Fixture("AgentReviewDraftId")));
    }

    [TestMethod]
    public async Task ListScenesReturnsTheSnapshotAndSceneIds()
    {
        var draft = Fixture("AgentReviewDraftId");
        var result = await new AgentActions(Context, Files).ListScenes(new() { VideoId = draft });

        Assert.AreEqual(draft, result.DraftVideoId);
        Assert.AreEqual(64, result.EditVersion.Length);
        Assert.IsTrue(result.Scenes.Any(scene => !string.IsNullOrWhiteSpace(scene.SceneId)));
    }

    [TestMethod]
    public async Task ReviseOneSceneReturnsAnAgentRun()
    {
        RequireWrites();
        var actions = new AgentActions(Context, Files);
        var draft = Fixture("AgentReviewDraftId");
        var snapshot = await actions.ListScenes(new() { VideoId = draft });
        var result = await actions.ReviseScenes(new()
        {
            AgentSessionId = Fixture("AgentSessionId"),
            DraftVideoId = snapshot.DraftVideoId,
            EditVersion = snapshot.EditVersion,
            SceneId = snapshot.Scenes.First().SceneId,
            Instruction = "Shorten the opening line and preserve the existing visual.",
            SubmissionKey = SubmissionKey()
        });

        Assert.IsFalse(string.IsNullOrWhiteSpace(result.RunId));
    }
}
