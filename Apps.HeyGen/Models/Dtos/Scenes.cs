using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class Scenes
{
    [JsonProperty("edit_version")]
    public string? EditVersion { get; set; }

    [JsonProperty("scenes")]
    public List<Scene> Items { get; set; } = [];
}
