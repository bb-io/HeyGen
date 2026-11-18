using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class Scene
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("script")]
    public List<ScriptEntry> Script { get; set; } = [];

    internal string Narration() => string.Join("\n", (Script ?? []).Where(entry =>
        entry.Type == "text" && !string.IsNullOrWhiteSpace(entry.Text)).Select(entry => entry.Text));
}
