using Apps.HeyGen.Events;
using Apps.HeyGen.Models.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.HeyGen.Base;

namespace Tests.HeyGen;

[TestClass]
public class ProofreadEventTests : TestBase
{
    [TestMethod]
    public async Task ScopedPollEmitsAnAlreadyReadySessionOnce()
    {
        var id = Fixture("CompletedProofreadId");
        var input = new ProofreadEventInput { ProofreadId = id };
        var events = new ProofreadPollingEvents(Context);
        var first = await events.OnProofreadReady(new() { PollingTime = DateTime.UtcNow }, input);

        Assert.IsTrue(first.FlyBird);
        Assert.AreEqual(id, first.Result!.Sessions.Single().ProofreadId);

        var second = await events.OnProofreadReady(new() { Memory = first.Memory, PollingTime = DateTime.UtcNow }, input);

        Assert.IsFalse(second.FlyBird);
    }

    [TestMethod]
    public async Task WorkspacePollStartsWithoutHistoricalEvents()
    {
        var result = await new ProofreadPollingEvents(Context).OnProofreadReady(
            new() { PollingTime = DateTime.UtcNow }, new());

        Assert.IsFalse(result.FlyBird);
        Assert.IsNotNull(result.Memory);
    }
}
