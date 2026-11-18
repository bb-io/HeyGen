using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.HeyGen.Api;

internal static class Requests
{
    public static string Required(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new PluginMisconfigurationException($"Enter {field}.");
        return value;
    }

    public static string Id(string? value, string field)
    {
        var id = Required(value, field).Trim();
        if (id is "." or ".." || id.Any(char.IsControl))
            throw new PluginMisconfigurationException($"Enter a valid value for {field}.");
        return Uri.EscapeDataString(id);
    }

    public static string SubmissionKey(string? value)
    {
        var key = Required(value, "a stable submission key").Trim();
        if (!Regex.IsMatch(key, @"\A[A-Za-z0-9_:.\-]{1,255}\z"))
            throw new PluginMisconfigurationException("Use 1–255 letters, digits, underscores, colons, periods or hyphens for the submission key.");
        return key;
    }

    public static string DeriveKey(string key, string purpose) =>
        $"heygen:{purpose}:{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key)))}";

    public static RestRequest Json(string path, Method method, Dictionary<string, object?> body)
    {
        var values = body.Where(x => x.Value is not null).ToDictionary(x => x.Key, x => x.Value);
        return new RestRequest(path, method).AddStringBody(JsonConvert.SerializeObject(values), DataFormat.Json);
    }

    public static T JsonInput<T>(string value, string field) where T : JToken
    {
        try
        {
            return JToken.Parse(value, new JsonLoadSettings { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error }) as T
                ?? throw new PluginMisconfigurationException($"Enter {field} as a JSON {(typeof(T) == typeof(JArray) ? "array" : "object")}.");
        }
        catch (JsonException)
        {
            throw new PluginMisconfigurationException($"Enter valid JSON for {field} with unique property names.");
        }
    }

    public static bool Matches(string? query, params string?[] values) =>
        string.IsNullOrWhiteSpace(query) ||
        values.Any(value => value?.Contains(query.Trim(), StringComparison.OrdinalIgnoreCase) == true);
}
