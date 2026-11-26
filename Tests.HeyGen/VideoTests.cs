using Apps.HeyGen.Actions;
using Apps.HeyGen.Models.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.HeyGen.Base;

namespace Tests.HeyGen;

[TestClass]
public class VideoTests : TestBase
{
    [TestMethod]
    public async Task SearchVideosReturnsAtMostRequestedCount()
    {
        var response = await new VideoActions(Context, Files).SearchVideos(new() { Maximum = 3 });
        Assert.IsTrue(response.Videos.Count <= 3);
    }

    [TestMethod]
    public async Task GetVideoReturnsFixture()
    {
        var id = Fixture("CompletedVideoId");
        var response = await new VideoActions(Context, Files).GetVideo(new() { VideoId = id });
        Assert.AreEqual(id, response.VideoId);
    }

    [TestMethod]
    public async Task DownloadVideoReturnsFile()
    {
        var response = await new VideoActions(Context, Files).DownloadVideo(new() { VideoId = Fixture("CompletedVideoId") });
        Assert.IsTrue(Files.SizeOf(response.File) > 0);
    }

    [TestMethod]
    public async Task GetCaptionsReturnsSrt()
    {
        var response = await new VideoActions(Context, Files).GetCaptions(new() { VideoId = Fixture("CompletedVideoId") });
        Assert.IsTrue(Files.SizeOf(response.File) > 0);
        StringAssert.EndsWith(response.File.Name, ".srt");
    }

    [TestMethod]
    public async Task CreateVideoReplaysOneJobForSameKey()
    {
        RequireWrites();
        var actions = new VideoActions(Context, Files);
        var input = new CreateVideoInput
        {
            AvatarId = Fixture("AvatarLookId"),
            Script = "This is a disposable integration test.",
            Title = "HeyGen integration test",
            SubmissionKey = SubmissionKey()
        };
        var first = await actions.CreateVideo(input);
        var replay = await actions.CreateVideo(input);
        Assert.IsFalse(string.IsNullOrWhiteSpace(first.VideoId));
        Assert.AreEqual(first.VideoId, replay.VideoId);
    }
}
