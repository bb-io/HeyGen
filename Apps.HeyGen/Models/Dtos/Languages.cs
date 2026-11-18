using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class Languages
{
    [JsonProperty("languages")]
    public List<string> All { get; set; } = [];

    [JsonProperty("available_on_current_plan")]
    public List<string>? Available { get; set; } = [];
}
