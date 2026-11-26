using System.Globalization;
using System.Net;
using Apps.HeyGen.Constants;
using Apps.HeyGen.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using Polly;
using RestSharp;

namespace Apps.HeyGen.Api;

public class HeyGenClient : BlackBirdRestClient
{
    private const int PageLimit = 50;
    private readonly string apiKey;

    public HeyGenClient(IEnumerable<AuthenticationCredentialsProvider> credentials) : this(credentials, 45_000)
    {
    }

    internal HeyGenClient(IEnumerable<AuthenticationCredentialsProvider> credentials, int timeoutMilliseconds)
        : base(new RestClientOptions
        {
            BaseUrl = new Uri("https://api.heygen.com/v3/"),
            FollowRedirects = false,
            MaxTimeout = timeoutMilliseconds
        })
    {
        apiKey = credentials?.FirstOrDefault(x => x.KeyName == CredsNames.ApiKey)?.Value?.Trim() ?? "";
        if (apiKey.Length == 0)
            throw new PluginMisconfigurationException("Enter a HeyGen API key from the API dashboard.");

        this.AddDefaultHeader(CredsNames.ApiKey, apiKey);
    }

    public override async Task<RestResponse> ExecuteWithErrorHandling(RestRequest request)
    {
        var policy = Policy.HandleResult<RestResponse>(response =>
                request.Method == Method.Get && response.StatusCode == HttpStatusCode.TooManyRequests
                && (RetryAfter(response) ?? TimeSpan.Zero) <= TimeSpan.FromSeconds(30))
            .WaitAndRetryAsync(3, (attempt, result, _) => RetryAfter(result.Result)
                ?? TimeSpan.FromMilliseconds(1000 * Math.Pow(2, attempt - 1) + Random.Shared.Next(250)),
                (_, _, _, _) => Task.CompletedTask);

        RestResponse response;
        try
        {
            response = await policy.ExecuteAsync(() => ExecuteAsync(request));
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or IOException)
        {
            throw TransportError(request.Method);
        }

        if (response.ResponseStatus is ResponseStatus.TimedOut or ResponseStatus.Aborted || (int)response.StatusCode == 0)
            throw TransportError(request.Method);

        if (!response.IsSuccessful)
            throw ConfigureErrorException(response);

        return response;
    }

    public async Task<T> Read<T>(RestRequest request) =>
        Read<T>(await ExecuteWithErrorHandling(request));

    internal static T Read<T>(RestResponse response)
    {
        try
        {
            var result = JsonConvert.DeserializeObject<T>(response.Content ?? "");
            return result ?? throw new PluginApplicationException("HeyGen returned an empty response. Check the HeyGen dashboard before repeating a write.");
        }
        catch (JsonException)
        {
            throw new PluginApplicationException("HeyGen returned an unreadable response. Check the HeyGen dashboard before repeating a write.");
        }
    }

    public async Task<T> Data<T>(RestRequest request)
    {
        var envelope = await Read<Envelope<T>>(request);
        return envelope.Data ?? throw new PluginApplicationException("HeyGen returned no data. Check the HeyGen dashboard before repeating a write.");
    }

    public async Task<List<T>> Paginate<T>(Func<RestRequest> createRequest, int pageSize,
        int? maximumMatches = null, Func<T, bool>? include = null, int maximumPages = PageLimit)
    {
        var items = new List<T>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        string? cursor = null;

        for (var pageNumber = 0; pageNumber < maximumPages; pageNumber++)
        {
            var request = createRequest().AddQueryParameter("limit", pageSize);
            if (cursor is not null)
                request.AddQueryParameter("token", cursor);

            var page = await Read<CursorPage<T>>(request);
            items.AddRange(include is null ? page.Data : page.Data.Where(include));

            if (maximumMatches is int count && items.Count >= count)
                return items.Take(count).ToList();

            cursor = page.NextToken;
            if (!page.HasMore && string.IsNullOrEmpty(cursor))
                return items;

            if (string.IsNullOrEmpty(cursor) || !seen.Add(cursor))
                throw new PluginApplicationException("HeyGen did not provide a usable next page. The search was incomplete.");
        }

        throw new PluginApplicationException("The HeyGen search reached its page limit. Narrow the search or enter a known ID.");
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var status = (int)response.StatusCode;
        var (code, error) = ReadError(response.Content);
        var detail = string.IsNullOrWhiteSpace(error) ? "" : $" {error}";
        return status switch
        {
            409 when code == "stale_edit_version" => new PluginMisconfigurationException(
                "The HeyGen draft changed after it was reviewed. List its scenes again and confirm the edit against the latest version."),
            400 or 409 or 422 => new PluginMisconfigurationException($"HeyGen rejected the inputs (HTTP {status}). Check the selected resources and parameters.{detail}"),
            401 => new PluginMisconfigurationException("HeyGen rejected the API key. Check whether the key is valid or has expired."),
            402 => new PluginMisconfigurationException($"HeyGen requires a funded API balance or a different plan.{detail}"),
            403 => new PluginMisconfigurationException($"HeyGen denied access. Check the API key scopes, workspace permissions and plan.{detail}"),
            404 => new PluginMisconfigurationException($"HeyGen could not find the resource. Check the ID and workspace access.{detail}"),
            429 => new PluginApplicationException($"HeyGen rate or concurrency limit reached. Wait before submitting more work.{detail}"),
            _ => new PluginApplicationException($"HeyGen request failed (HTTP {status}). Check job history before repeating a write.{detail}")
        };
    }

    private static Exception TransportError(Method method) => new PluginApplicationException(
        method == Method.Get
            ? "Could not read from HeyGen. Check connectivity and HeyGen service status."
            : "The HeyGen write has an unknown outcome. Check job or resource history before repeating it.");

    private (string? Code, string? Message) ReadError(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return (null, null);

        try
        {
            var error = JsonConvert.DeserializeObject<ErrorEnvelope>(content);
            var code = error?.Error?.Code;
            var message = error?.Error?.Message ?? error?.Message;
            if (string.IsNullOrWhiteSpace(message))
                return (code, null);

            message = message.Replace(apiKey, "[redacted]", StringComparison.Ordinal);
            return (code, message.Length <= 280 ? message : message[..280]);
        }
        catch (JsonException)
        {
            return (null, null);
        }
    }

    private static TimeSpan? RetryAfter(RestResponse response)
    {
        var value = response.Headers?.FirstOrDefault(x =>
            string.Equals(x.Name, "Retry-After", StringComparison.OrdinalIgnoreCase))?.Value?.ToString();
        if (int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var seconds))
            return TimeSpan.FromSeconds(seconds);

        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var until))
            return until > DateTimeOffset.UtcNow ? until - DateTimeOffset.UtcNow : TimeSpan.Zero;

        return null;
    }
}
