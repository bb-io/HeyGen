using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Handlers.Static;

public class AvatarAspectHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("auto", "Match source"),
        new("16:9", "Landscape (16:9)"),
        new("9:16", "Portrait (9:16)"),
        new("4:5", "Portrait (4:5)"),
        new("5:4", "Landscape (5:4)"),
        new("1:1", "Square (1:1)")
    ];
}
