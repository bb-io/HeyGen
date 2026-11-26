using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class Envelope<T>
{
    [JsonProperty("data")]
    public T? Data { get; set; }
}
