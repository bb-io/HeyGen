using Apps.HeyGen.Actions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.HeyGen.Base;

namespace Tests.HeyGen;

[TestClass]
public class StudioVideoTests : TestBase
{
    [TestMethod]
    public async Task CreateStudioVideoReturnsSingleVideoId()
    {
        RequireWrites();
        var result = await new StudioVideoActions(Context, Files).CreateStudioVideo(new()
        {
            Title = "Disposable multi-scene Studio test",
            SubmissionKey = SubmissionKey(),
            Scenes =
            [
                new() { Type = "avatar_video", AvatarId = Fixture("AvatarLookId"), Script = "Studio integration test." },
                new() { Type = "image", MediaUrl = Fixture("StudioImageUrl"), Duration = 2 },
                new() { Type = "video", MediaUrl = Fixture("VideoSourceUrl"), MuteClip = true }
            ]
        });

        Assert.IsFalse(string.IsNullOrWhiteSpace(result.VideoId));
    }
}
