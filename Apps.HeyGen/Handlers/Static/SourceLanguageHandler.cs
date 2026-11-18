using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Handlers.Static;

public class SourceLanguageHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("ar", "Arabic"),
        new("zh", "Chinese"),
        new("nl", "Dutch"),
        new("en", "English"),
        new("fr", "French"),
        new("de", "German"),
        new("hi", "Hindi"),
        new("hu", "Hungarian"),
        new("it", "Italian"),
        new("ja", "Japanese"),
        new("ko", "Korean"),
        new("pl", "Polish"),
        new("pt", "Portuguese"),
        new("es", "Spanish"),
        new("tr", "Turkish"),
        new("uk", "Ukrainian")
    ];
}
