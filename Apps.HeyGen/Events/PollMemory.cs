namespace Apps.HeyGen.Events;

public class PollMemory
{
    public DateTime LastScan { get; set; }

    public DateTime Started { get; set; }

    public string? ResourceId { get; set; }

    public List<string> Pending { get; set; } = [];

    public List<string> SeenTerminal { get; set; } = [];
}
