using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Handlers.Static;

public class ClipDurationHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("30", "20–30 seconds"),
        new("60", "30–60 seconds"),
        new("180", "60–120 seconds"),
        new("long", "Over 120 seconds")
    ];
}
