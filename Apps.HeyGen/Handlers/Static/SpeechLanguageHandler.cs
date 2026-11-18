using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Handlers.Static;

public class SpeechLanguageHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("ar", "Arabic"),
        new("be", "Belarusian"),
        new("bg", "Bulgarian"),
        new("ca", "Catalan"),
        new("zh", "Chinese"),
        new("hr", "Croatian"),
        new("cs", "Czech"),
        new("da", "Danish"),
        new("nl", "Dutch"),
        new("en", "English"),
        new("fi", "Finnish"),
        new("fr", "French"),
        new("de", "German"),
        new("el", "Greek"),
        new("he", "Hebrew"),
        new("hi", "Hindi"),
        new("hu", "Hungarian"),
        new("id", "Indonesian"),
        new("it", "Italian"),
        new("ja", "Japanese"),
        new("ko", "Korean"),
        new("mk", "Macedonian"),
        new("ms", "Malay"),
        new("fa", "Persian"),
        new("pl", "Polish"),
        new("pt", "Portuguese"),
        new("ro", "Romanian"),
        new("ru", "Russian"),
        new("sr", "Serbian"),
        new("sk", "Slovak"),
        new("sl", "Slovenian"),
        new("es", "Spanish"),
        new("sv", "Swedish"),
        new("tl", "Tagalog"),
        new("ta", "Tamil"),
        new("th", "Thai"),
        new("tr", "Turkish"),
        new("uk", "Ukrainian"),
        new("vi", "Vietnamese")
    ];
}
