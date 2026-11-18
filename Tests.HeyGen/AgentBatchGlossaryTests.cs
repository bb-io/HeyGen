using Apps.HeyGen.Actions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Tests.HeyGen.Base;

namespace Tests.HeyGen;

[TestClass]
public class AgentBatchGlossaryTests : TestBase
{
    [TestMethod]
    public async Task CreateSessionReturnsSessionId()
    {
        RequireWrites();
        var result = await new AgentActions(Context, Files).CreateSession(new()
        {
            Prompt = "Make a short private integration test video about a green notebook."
        });
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.AgentSessionId));
    }

    [TestMethod]
    public async Task GetSessionReturnsFixture()
    {
        var id = Fixture("AgentSessionId");
        var result = await new AgentActions(Context, Files).GetSession(new() { AgentSessionId = id });
        Assert.AreEqual(id, result.AgentSessionId);
    }

    [TestMethod]
    public async Task CreateVideoBatchReturnsBatchId()
    {
        RequireWrites();
        var body = JsonConvert.SerializeObject(new[]
        {
            new { type = "avatar", avatar_id = Fixture("AvatarLookId"), script = "This is a disposable batch test." }
        });
        var result = await new BatchActions(Context).CreateBatch(new() { BatchType = "videos", Items = body, SubmissionKey = SubmissionKey() });
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.BatchId));
    }

    [TestMethod]
    public async Task GetBatchReturnsFixture()
    {
        var id = Fixture("ExistingVideoBatchId");
        var result = await new BatchActions(Context).GetBatch(new() { BatchId = id, BatchType = "videos" });
        Assert.AreEqual(id, result.BatchId);
    }

    [TestMethod]
    public async Task CreateAssetBatchReturnsBatchId()
    {
        RequireWrites();
        var result = await new AssetBatchActions(Context, Files).CreateBatch(new()
        {
            Files = [FixtureFile("AssetFilePath")], SubmissionKey = SubmissionKey()
        });
        Assert.AreEqual("assets", result.BatchType);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.BatchId));
    }

    [TestMethod]
    public async Task CompleteAssetBatchReturnsSameId()
    {
        RequireWrites();
        var id = Fixture("UploadedAssetBatchId");
        var result = await new AssetBatchActions(Context, Files).CompleteBatch(new() { BatchId = id });
        Assert.AreEqual(id, result.BatchId);
    }

    [TestMethod]
    public async Task SearchGlossariesReturnsCollection()
    {
        Assert.IsNotNull((await new GlossaryActions(Context).SearchGlossaries()).BrandGlossaries);
    }

    [TestMethod]
    public async Task GetGlossaryReturnsFixture()
    {
        var id = Fixture("GlossaryId");
        var result = await new GlossaryActions(Context).GetGlossary(new() { BrandGlossaryId = id });
        Assert.AreEqual(id, result.BrandGlossaryId);
    }

    [TestMethod]
    public async Task CreateUpdateAndDeleteDisposableGlossary()
    {
        RequireWrites();
        var actions = new GlossaryActions(Context);
        var key = SubmissionKey();
        var created = await actions.CreateGlossary(new()
        {
            Name = $"HeyGen test {key}", SubmissionKey = key, DoNotTranslate = """[{"term":"Blackbird"}]"""
        });
        Assert.IsFalse(string.IsNullOrWhiteSpace(created.BrandGlossaryId));
        var input = new Apps.HeyGen.Models.Inputs.GlossaryInput { BrandGlossaryId = created.BrandGlossaryId };
        try
        {
            var updated = await actions.UpdateGlossary(input, new() { ForcedTranslations = "[]" });
            Assert.AreEqual(input.BrandGlossaryId, updated.BrandGlossaryId);
            StringAssert.Contains(updated.DoNotTranslate, "Blackbird");
        }
        finally
        {
            var deleted = await actions.DeleteGlossary(input);
            Assert.AreEqual(input.BrandGlossaryId, deleted.BrandGlossaryId);
        }
    }
}
