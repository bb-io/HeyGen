using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Handlers.Static;

public class VideoStatusHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("pending", "Pending"),
        new("processing", "Processing"),
        new("completed", "Completed"),
        new("failed", "Failed")
    ];
}
