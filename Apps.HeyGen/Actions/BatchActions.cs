using Apps.HeyGen.Api;
using Apps.HeyGen.Models.Dtos;
using Apps.HeyGen.Models.Inputs;
using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.HeyGen.Actions;

[ActionList("Batches")]
public class BatchActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Create batch", Description = "Submit 1–100 video, translation or lipsync requests as a native HeyGen batch.")]
    public async Task<BatchSubmission> CreateBatch([ActionParameter] BatchCreateInput input)
    {
        var key = Requests.SubmissionKey(input.SubmissionKey);
        var wireField = input.BatchType switch
        {
            "videos" => "videos",
            "video-translations" => "video_translations",
            "lipsyncs" => "lipsyncs",
            _ => throw new PluginMisconfigurationException("Choose a video, video translation or lipsync batch.")
        };
        var items = Requests.JsonInput<JArray>(Requests.Required(input.Items, "the batch job inputs"), "batch job inputs");
        if (items.Count is < 1 or > 100 || items.Any(x => x.Type != JTokenType.Object))
            throw new PluginMisconfigurationException("Provide 1–100 job objects in the batch JSON array.");

        var request = Requests.Json($"{input.BatchType}/batches", Method.Post, new()
        {
            ["title"] = input.Title,
            [wireField] = items
        }).AddHeader("Idempotency-Key", key);

        var result = await Client.Data<BatchSubmission>(request);
        result.BatchType = input.BatchType;
        return result;
    }

    [Action("Get batch", Description = "Read aggregate status, IDs and individual outcomes for a HeyGen batch.")]
    public async Task<BatchDetails> GetBatch([ActionParameter] BatchInput input)
    {
        var path = input.BatchType switch
        {
            "videos" or "video-translations" or "lipsyncs" or "assets" => input.BatchType,
            _ => throw new PluginMisconfigurationException("Select the batch type returned by the create action.")
        };
        var id = Requests.Id(input.BatchId, "a batch ID");
        var cursor = (string?)null;
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var result = new BatchDetails();

        for (var page = 0; page < 50; page++)
        {
            var request = new RestRequest($"{path}/batches/{id}").AddQueryParameter("limit", 100);
            if (cursor is not null)
                request.AddQueryParameter("token", cursor);

            var response = await Client.Data<BatchPage>(request);
            if (page == 0)
            {
                result.BatchId = response.BatchId;
                result.Title = response.Title;
                result.Status = response.Status;
                result.TotalItems = response.TotalItems;
            }

            result.Items.AddRange(response.Items.Select(x => new BatchItem
            {
                Index = x.Index,
                JobId = x.JobId,
                Status = x.Status,
                Failure = x.Error?.Message
            }));

            if (!response.HasMore)
                return result;

            cursor = response.NextToken;
            if (string.IsNullOrEmpty(cursor) || !seen.Add(cursor))
                throw new PluginApplicationException("HeyGen did not provide a usable next page of batch items.");
        }

        throw new PluginApplicationException("HeyGen batch status exceeded the page limit. Inspect the batch in HeyGen.");
    }
}
