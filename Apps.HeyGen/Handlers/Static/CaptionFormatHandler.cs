using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Handlers.Static;

public class CaptionFormatHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("srt", "SRT"),
        new("vtt", "WebVTT")
    ];
}
