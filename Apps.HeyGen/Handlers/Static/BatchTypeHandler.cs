using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Handlers.Static;

public class BatchTypeHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("videos", "Avatar videos and other v3 video types"),
        new("video-translations", "Video translations"),
        new("lipsyncs", "Lipsync jobs")
    ];
}
