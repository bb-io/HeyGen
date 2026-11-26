using Apps.HeyGen.Actions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.HeyGen.Base;

namespace Tests.HeyGen;

[TestClass]
public class ProofreadTests : TestBase
{
    [TestMethod]
    public async Task StartProofreadReturnsSessionId()
    {
        RequireWrites();
        var result = await new ProofreadActions(Context, Files).StartProofread(new()
        {
            VideoUrl = Fixture("VideoSourceUrl"),
            Title = "Disposable translation proofread",
            Languages = ["Spanish"],
            SubmissionKey = SubmissionKey()
        });

        Assert.AreEqual(1, result.ProofreadIds.Count);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.ProofreadIds[0]));
    }

    [TestMethod]
    public async Task GetProofreadReturnsCompletedSession()
    {
        var id = Fixture("CompletedProofreadId");
        var result = await new ProofreadActions(Context, Files).GetProofread(new() { ProofreadId = id });

        Assert.AreEqual(id, result.ProofreadId);
        Assert.AreEqual("completed", result.Status);
    }

    [TestMethod]
    public async Task DownloadProofreadReturnsSrtFile()
    {
        var result = await new ProofreadActions(Context, Files).DownloadProofreadSrt(
            new() { ProofreadId = Fixture("CompletedProofreadId") });

        Assert.IsTrue(Files.SizeOf(result.File) > 0);
        StringAssert.EndsWith(result.File.Name, ".srt");
    }

    [TestMethod]
    public async Task UploadProofreadReplacesSubtitlesOnDisposableSession()
    {
        RequireWrites();
        var id = Fixture("EditableProofreadId");
        var result = await new ProofreadActions(Context, Files).UploadProofreadSrt(new()
        {
            ProofreadId = id,
            File = FixtureFile("ApprovedProofreadSrtPath")
        });

        Assert.AreEqual(id, result.ProofreadId);
    }

    [TestMethod]
    public async Task RenderProofreadReturnsTranslationId()
    {
        RequireWrites();
        var result = await new ProofreadActions(Context, Files).RenderProofread(new()
        {
            ProofreadId = Fixture("RenderableProofreadId"),
            SubmissionKey = SubmissionKey()
        });

        Assert.IsFalse(string.IsNullOrWhiteSpace(result.TranslationId));
    }
}
