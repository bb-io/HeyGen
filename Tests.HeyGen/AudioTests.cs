using Apps.HeyGen.Actions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.HeyGen.Base;

namespace Tests.HeyGen;

[TestClass]
public class AudioTests : TestBase
{
    [TestMethod]
    public async Task TextToSpeechReturnsMp3()
    {
        RequireWrites();
        var result = await new AudioActions(Context, Files).TextToSpeech(new()
        {
            VoiceId = Fixture("TtsVoiceId"),
            Text = "This is a disposable narration test.",
            Language = "en",
            Format = "mp3"
        });

        Assert.IsTrue(result.Duration > 0);
        Assert.IsTrue(Files.SizeOf(result.File) > 0);
        Assert.AreEqual("audio/mpeg", result.File.ContentType);
        StringAssert.EndsWith(result.File.Name, ".mp3");
    }

    [TestMethod]
    public async Task SpeechToSpeechReturnsWav()
    {
        RequireWrites();
        var result = await new AudioActions(Context, Files).SpeechToSpeech(new()
        {
            VoiceId = Fixture("InstantVoiceId"),
            AudioFile = FixtureFile("NarrationFilePath"),
            SubmissionKey = SubmissionKey()
        });

        Assert.IsTrue(result.Duration > 0);
        Assert.IsTrue(Files.SizeOf(result.File) > 0);
        Assert.AreEqual("audio/wav", result.File.ContentType);
        StringAssert.EndsWith(result.File.Name, ".wav");
    }

    [TestMethod]
    public async Task CreateAvatarVideoFromAudioReturnsVideoId()
    {
        RequireWrites();
        var result = await new AudioActions(Context, Files).CreateVideoFromAudio(
            new()
            {
                AvatarId = Fixture("AvatarLookId"),
                SubmissionKey = SubmissionKey()
            },
            new() { AudioFile = FixtureFile("NarrationFilePath") });

        Assert.IsFalse(string.IsNullOrWhiteSpace(result.VideoId));
    }
}
