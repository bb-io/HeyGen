namespace Apps.HeyGen.Events;

internal static class JobPolling
{
    private static readonly TimeSpan Overlap = TimeSpan.FromMinutes(5);

    public static (PollMemory Memory, List<T> Matches) Detect<T>(List<T> items, PollMemory? prior,
        DateTime now, string? resourceId, string desiredStatus, Func<T, string> id, Func<T, string> status,
        Func<T, DateTime?> created)
    {
        var baseline = prior is null || !string.Equals(prior.ResourceId, resourceId, StringComparison.Ordinal);
        var previousScan = prior?.LastScan ?? now;
        var scan = now < previousScan && !baseline ? previousScan : now;
        var started = baseline ? now : prior!.Started;
        var eligibleSince = baseline ? now : Max(started, previousScan - Overlap);
        var knownPending = baseline ? new HashSet<string>(StringComparer.Ordinal) : prior!.Pending.ToHashSet(StringComparer.Ordinal);
        var knownTerminal = baseline ? new HashSet<string>(StringComparer.Ordinal) : prior!.SeenTerminal.ToHashSet(StringComparer.Ordinal);
        var matches = new List<T>();

        foreach (var item in items.Where(item => !string.IsNullOrWhiteSpace(id(item))).DistinctBy(id))
        {
            var itemId = id(item);
            var itemStatus = status(item);
            if (itemStatus is not ("completed" or "failed"))
            {
                knownPending.Add(itemId);
                continue;
            }

            var createdAt = created(item);
            var mayBeNew = createdAt >= eligibleSince;
            var transitioned = knownPending.Remove(itemId);
            var alreadySeen = knownTerminal.Contains(itemId);
            if (!baseline && !alreadySeen && (transitioned || mayBeNew) && itemStatus == desiredStatus)
                matches.Add(item);

            if (itemStatus == desiredStatus)
                knownTerminal.Add(itemId);
        }

        var next = new PollMemory
        {
            Started = started,
            LastScan = scan,
            ResourceId = resourceId,
            Pending = knownPending.TakeLast(10_000).ToList(),
            SeenTerminal = knownTerminal.TakeLast(10_000).ToList()
        };
        return (next, matches);
    }

    private static DateTime Max(DateTime first, DateTime second) => first > second ? first : second;
}
