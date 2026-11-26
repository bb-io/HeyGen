using Apps.HeyGen.Api;
using Apps.HeyGen.Models.Inputs;
using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.HeyGen.Events;

[PollingEventList]
public class PollingEvents(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [PollingEvent("On video completed", Description = "Detect newly completed HeyGen videos. Results from one poll are sent together.")]
    public Task<PollingEventResponse<PollMemory, VideoResults>> OnVideoCompleted(
        PollingEventRequest<PollMemory> request, [PollingEventParameter] VideoEventInput input) =>
        PollVideos(request, input?.VideoId, "completed");

    [PollingEvent("On video failed", Description = "Detect newly failed HeyGen videos. Results from one poll are sent together.")]
    public Task<PollingEventResponse<PollMemory, VideoResults>> OnVideoFailed(
        PollingEventRequest<PollMemory> request, [PollingEventParameter] VideoEventInput input) =>
        PollVideos(request, input?.VideoId, "failed");

    [PollingEvent("On translation completed", Description = "Detect newly completed native translations. Results from one poll are sent together.")]
    public async Task<PollingEventResponse<PollMemory, TranslationResults>> OnTranslationCompleted(
        PollingEventRequest<PollMemory> request, [PollingEventParameter] TranslationEventInput input)
    {
        var now = DateTime.UtcNow;
        var id = Filter(input?.TranslationId);
        var items = await GetItems<TranslationDetails>("video-translations", id);
        var (memory, matches) = JobPolling.Detect(items, request.Memory, now, id, "completed",
            x => x.TranslationId, x => x.Status, x => x.CreatedAt);

        return new()
        {
            Memory = memory,
            FlyBird = matches.Count > 0,
            Result = matches.Count == 0 ? null : new TranslationResults { Translations = matches }
        };
    }

    private async Task<PollingEventResponse<PollMemory, VideoResults>> PollVideos(
        PollingEventRequest<PollMemory> request, string? filter, string status)
    {
        var now = DateTime.UtcNow;
        var id = Filter(filter);
        var items = await GetItems<VideoDetails>("videos", id);
        var (memory, matches) = JobPolling.Detect(items, request.Memory, now, id, status,
            x => x.VideoId, x => x.Status, x => x.CreatedAt);

        return new()
        {
            Memory = memory,
            FlyBird = matches.Count > 0,
            Result = matches.Count == 0 ? null : new VideoResults { Videos = matches }
        };
    }

    private async Task<List<T>> GetItems<T>(string path, string? id) =>
        id is null
            ? await Client.Paginate<T>(() => new RestRequest(path), 100)
            : [await Client.Data<T>(new RestRequest($"{path}/{Requests.Id(id, "a job ID")}"))];

    private static string? Filter(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
