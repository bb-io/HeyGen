using Apps.HeyGen.Connections;
using Apps.HeyGen.Events;
using Apps.HeyGen.Handlers;
using Blackbird.Applications.Sdk.Common.Polling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.HeyGen.Base;

namespace Tests.HeyGen;

[TestClass]
public class ConnectionAndEventTests : TestBase
{
    [TestMethod]
    public async Task ConnectionValidatesKey()
    {
        var result = await new ConnectionValidator(Context).ValidateConnection(Context.AuthenticationCredentialsProviders, default);
        Assert.IsTrue(result.IsValid, result.Message);
    }

    [TestMethod]
    public async Task PickersReturnProviderValues()
    {
        var videos = await new VideoHandler(Context).GetDataAsync(new(), default);
        var languages = await new LanguageHandler(Context).GetDataAsync(new(), default);
        var voices = await new VoiceHandler(Context).GetDataAsync(new(), default);
        var avatars = await new AvatarHandler(Context, new() { AvatarGroupId = Fixture("AvatarGroupId") }).GetDataAsync(new(), default);
        var templates = await new TemplateHandler(Context).GetDataAsync(new(), default);
        Assert.IsTrue(videos.All(x => !string.IsNullOrWhiteSpace(x.Value)));
        Assert.IsTrue(languages.Any());
        Assert.IsTrue(voices.Any());
        Assert.IsTrue(avatars.Any());
        Assert.IsTrue(templates.All(x => !string.IsNullOrWhiteSpace(x.Value)));
    }

    [TestMethod]
    public async Task VideoCompletedBaselinesAnAlreadyCompletedJob()
    {
        var events = new PollingEvents(Context);
        var input = new Apps.HeyGen.Models.Inputs.VideoEventInput { VideoId = Fixture("CompletedVideoId") };
        var baseline = await events.OnVideoCompleted(new() { PollingTime = DateTime.UtcNow }, input);
        Assert.IsFalse(baseline.FlyBird);
        var next = await events.OnVideoCompleted(new() { Memory = baseline.Memory, PollingTime = DateTime.UtcNow }, input);
        Assert.IsFalse(next.FlyBird);
    }

    [TestMethod]
    public async Task VideoFailedBaselinesWithoutBackfill()
    {
        var result = await new PollingEvents(Context).OnVideoFailed(
            new PollingEventRequest<PollMemory> { PollingTime = DateTime.UtcNow }, new() { VideoId = Fixture("CompletedVideoId") });
        Assert.IsFalse(result.FlyBird);
    }

    [TestMethod]
    public async Task TranslationCompletedBaselinesWithoutBackfill()
    {
        var result = await new PollingEvents(Context).OnTranslationCompleted(
            new PollingEventRequest<PollMemory> { PollingTime = DateTime.UtcNow }, new() { TranslationId = Fixture("CompletedTranslationId") });
        Assert.IsFalse(result.FlyBird);
    }
}
