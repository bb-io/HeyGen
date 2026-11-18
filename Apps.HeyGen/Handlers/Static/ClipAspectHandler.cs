using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Handlers.Static;

public class ClipAspectHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("portrait", "Portrait (9:16)"),
        new("landscape", "Landscape (16:9)"),
        new("square", "Square (1:1)")
    ];
}
