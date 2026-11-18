using Apps.HeyGen.Api;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Handlers;

internal static class Picker
{
    public const int Limit = 40;
    public const int Pages = 15;

    public static bool Matches(string? query, string id, string? name, string? detail = null) =>
        Requests.Matches(query, id, name, detail);

    public static DataSourceItem Item(string id, string? name, string? detail = null)
    {
        var suffix = string.IsNullOrWhiteSpace(detail) ? id : $"{detail} · {id}";
        return new DataSourceItem(id, string.IsNullOrWhiteSpace(name) ? suffix : $"{name} ({suffix})");
    }
}
