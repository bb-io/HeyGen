using System.Globalization;
using Apps.HeyGen.Api;
using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class VideoHandler(InvocationContext invocationContext)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    private const int DetailLookupLimit = 5;

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var items = await Client.Paginate<VideoDetails>(() => CreateRequest(context),
            Picker.Limit, Picker.Limit, video => !string.IsNullOrWhiteSpace(video.VideoId), Picker.Pages);
        var videos = items.DistinctBy(video => video.VideoId).ToList();
        var titles = videos.ToDictionary(video => video.VideoId, GetTitle, StringComparer.Ordinal);
        foreach (var video in videos.Where(video => titles[video.VideoId] is null).Take(DetailLookupLimit))
        {
            try
            {
                var id = Requests.Id(video.VideoId, "a video ID");
                var details = await Client.Data<VideoDetails>(new RestRequest($"videos/{id}"));
                titles[video.VideoId] = GetTitle(details, video.VideoId);
                video.CreatedAt ??= details.CreatedAt;
                video.Duration ??= details.Duration;
            }
            catch (PluginMisconfigurationException)
            {
                continue;
            }
            catch (PluginApplicationException)
            {
                break;
            }
        }

        var duplicateTitles = titles.Values.OfType<string>()
            .GroupBy(title => title, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return videos.Select(video =>
        {
            var title = titles[video.VideoId];
            return new DataSourceItem(video.VideoId, FormatLabel(video, title,
                title is null || duplicateTitles.Contains(title)));
        });
    }

    private static string? GetTitle(VideoDetails video) => GetTitle(video, video.VideoId);

    private static string? GetTitle(VideoDetails video, string videoId)
    {
        var title = video.Title?.Trim();
        return string.IsNullOrWhiteSpace(title) || string.Equals(title, videoId, StringComparison.OrdinalIgnoreCase)
            ? null
            : title;
    }

    private static string FormatLabel(VideoDetails video, string? title, bool showId)
    {
        var label = new List<string> { title ?? "Untitled video" };

        if (video.CreatedAt is DateTime createdAt)
            label.Add(createdAt.ToString("d MMM yyyy", CultureInfo.InvariantCulture));

        if (video.Duration is > 0 and < 8_640_000)
        {
            var duration = TimeSpan.FromSeconds(Math.Round(video.Duration.Value, MidpointRounding.AwayFromZero));
            label.Add(duration.TotalHours >= 1
                ? FormattableString.Invariant($"{(int)duration.TotalHours}:{duration.Minutes:00}:{duration.Seconds:00}")
                : FormattableString.Invariant($"{duration.Minutes}:{duration.Seconds:00}"));
        }

        if (!string.IsNullOrWhiteSpace(video.Status) && !video.Status.Equals("completed", StringComparison.OrdinalIgnoreCase))
            label.Add(video.Status);

        if (showId)
            label.Add(video.VideoId.Length > 8 ? $"…{video.VideoId[^8..]}" : video.VideoId);

        return string.Join(" · ", label);
    }

    private static RestRequest CreateRequest(DataSourceContext context)
    {
        var request = new RestRequest("videos");
        return string.IsNullOrWhiteSpace(context.SearchString) ? request : request.AddQueryParameter("title", context.SearchString.Trim());
    }
}
