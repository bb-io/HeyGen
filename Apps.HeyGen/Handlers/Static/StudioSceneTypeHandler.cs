using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Handlers.Static;

public class StudioSceneTypeHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("avatar_video", "Speaking avatar"),
        new("image", "Still image"),
        new("video", "Video clip")
    ];
}
