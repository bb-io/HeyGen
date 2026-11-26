using Newtonsoft.Json;

namespace Apps.HeyGen.Models.Dtos;

public class Glossary
{
    [JsonProperty("brand_glossary_id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("terms")]
    public List<GlossaryTerm> Terms { get; set; } = [];

    [JsonProperty("do_not_translate_terms")]
    public List<GlossaryTerm> DoNotTranslate { get; set; } = [];

    [JsonProperty("forced_translations")]
    public List<GlossaryTerm> ForcedTranslations { get; set; } = [];
}
