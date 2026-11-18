using Apps.HeyGen.Api;
using Apps.HeyGen.Models.Inputs;
using Apps.HeyGen.Models.Outputs;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.HeyGen.Events;

[PollingEventList]
public class ProofreadPollingEvents(InvocationContext invocationContext) : Invocable(invocationContext)
{
    private const string Sessions = "video-translations/proofreads";

    [PollingEvent("On proofread ready", Description = "Wait for editable translated subtitles. Scope to a session to use as a checkpoint.")]
    public async Task<PollingEventResponse<PollMemory, ProofreadResults>> OnProofreadReady(
        PollingEventRequest<PollMemory> request, [PollingEventParameter] ProofreadEventInput input)
    {
        var now = DateTime.UtcNow;
        var id = string.IsNullOrWhiteSpace(input?.ProofreadId) ? null : input.ProofreadId.Trim();
        var prior = request.Memory;
        List<ProofreadDetails> sessions;
        if (id is null)
        {
            sessions = await Client.Paginate<ProofreadDetails>(() => new RestRequest(Sessions), 100);
        }
        else
        {
            var pathId = Requests.Id(id, "a proofread session ID");
            var session = await Client.Data<ProofreadDetails>(new RestRequest($"{Sessions}/{pathId}"));
            if (session.Status == "failed")
                throw new PluginApplicationException($"HeyGen could not prepare subtitles for proofread session {id}. {session.FailureMessage}");

            sessions = [session];
            if (prior?.ResourceId != id)
                prior = new PollMemory { ResourceId = id, Started = now, LastScan = now, Pending = [id] };
        }

        var (memory, matches) = JobPolling.Detect(sessions, prior, now, id, "completed",
            session => session.ProofreadId, session => session.Status, session => session.CreatedAt);

        return new()
        {
            Memory = memory,
            FlyBird = matches.Count > 0,
            Result = matches.Count == 0 ? null : new ProofreadResults { Sessions = matches }
        };
    }
}
