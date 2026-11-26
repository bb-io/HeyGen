using Apps.HeyGen.Actions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.HeyGen.Base;

namespace Tests.HeyGen;

[TestClass]
public class MediaJobTests : TestBase
{
    [TestMethod]
    public async Task TranslateVideoReturnsTranslationId()
    {
        RequireWrites();
        var result = await new TranslationActions(Context, Files).TranslateVideo(new()
        {
            VideoUrl = Fixture("VideoSourceUrl"), Languages = ["English"], SubmissionKey = SubmissionKey()
        });
        Assert.AreEqual(1, result.TranslationIds.Count);
    }

    [TestMethod]
    public async Task GetTranslationReturnsFixture()
    {
        var id = Fixture("CompletedTranslationId");
        Assert.AreEqual(id, (await new TranslationActions(Context, Files).GetTranslation(new() { TranslationId = id })).TranslationId);
    }

    [TestMethod]
    public async Task DownloadTranslationReturnsFile()
    {
        var file = await new TranslationActions(Context, Files).DownloadTranslation(new() { TranslationId = Fixture("CompletedTranslationId") });
        Assert.IsTrue(Files.SizeOf(file.File) > 0);
    }

    [TestMethod]
    public async Task GetTranslationCaptionsReturnsFile()
    {
        var file = await new TranslationActions(Context, Files).GetTranslationCaptions(new() { TranslationId = Fixture("CompletedTranslationId") });
        Assert.IsTrue(Files.SizeOf(file.File) > 0);
    }

    [TestMethod]
    public async Task CreateLipsyncReturnsJobId()
    {
        RequireWrites();
        var result = await new LipsyncActions(Context, Files).CreateLipsync(new()
        {
            VideoUrl = Fixture("VideoSourceUrl"), AudioUrl = Fixture("AudioSourceUrl"), SubmissionKey = SubmissionKey()
        });
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.LipsyncId));
    }

    [TestMethod]
    public async Task GetLipsyncReturnsFixture()
    {
        var id = Fixture("CompletedLipsyncId");
        Assert.AreEqual(id, (await new LipsyncActions(Context, Files).GetLipsync(new() { LipsyncId = id })).LipsyncId);
    }

    [TestMethod]
    public async Task DownloadLipsyncReturnsFile()
    {
        var file = await new LipsyncActions(Context, Files).DownloadLipsync(new() { LipsyncId = Fixture("CompletedLipsyncId") });
        Assert.IsTrue(Files.SizeOf(file.File) > 0);
    }

    [TestMethod]
    public async Task CreateClippingReturnsJobId()
    {
        RequireWrites();
        var result = await new ClippingActions(Context, Files).CreateClipping(new()
        {
            VideoUrl = Fixture("VideoSourceUrl"), Durations = ["30"], SubmissionKey = SubmissionKey()
        });
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.ClippingJobId));
    }

    [TestMethod]
    public async Task GetClippingReturnsFixture()
    {
        var id = Fixture("CompletedClippingJobId");
        var result = await new ClippingActions(Context, Files).GetClipping(new() { ClippingJobId = id });
        Assert.AreEqual(id, result.ClippingJobId);
    }

    [TestMethod]
    public async Task DownloadClipReturnsFile()
    {
        var result = await new ClippingActions(Context, Files).DownloadClip(new()
        {
            ClippingJobId = Fixture("CompletedClippingJobId"), ClipId = Fixture("CompletedClipId")
        });
        Assert.IsTrue(Files.SizeOf(result.File) > 0);
    }
}
