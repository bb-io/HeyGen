using Apps.HeyGen.Models.Dtos;
using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common.Exceptions;
using RestSharp;

namespace Apps.HeyGen.Api;

internal static class AgentDrafts
{
    public static async Task<VideoResults> Read(HeyGenClient client, string sessionId)
    {
        var id = Requests.Id(sessionId, "an Agent session ID");
        var response = await client.Read<CursorPage<VideoDetails>>(new RestRequest($"video-agents/{id}/videos"));
        if (response.HasMore)
            throw new PluginApplicationException("HeyGen returned a partial draft list without documented pagination for this endpoint.");

        return new VideoResults
        {
            Videos = response.Data.Where(draft => !string.IsNullOrWhiteSpace(draft.VideoId))
                .DistinctBy(draft => draft.VideoId)
                .ToList()
        };
    }
}
