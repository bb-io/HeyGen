using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Apps.HeyGen.Models.Outputs;

public class TranslationDetails
{
    [Display("Translation ID")]
    [JsonProperty("id")]
    public string TranslationId { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string? Title { get; set; }

    [Display("Status")]
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [Display("Source language")]
    [JsonProperty("input_language")]
    public string? InputLanguage { get; set; }

    [Display("Target language")]
    [JsonProperty("output_language")]
    public string? OutputLanguage { get; set; }

    [Display("Video URL")]
    [JsonProperty("video_url")]
    public string? VideoUrl { get; set; }

    [Display("Audio URL")]
    [JsonProperty("audio_url")]
    public string? AudioUrl { get; set; }

    [Display("SRT URL")]
    [JsonProperty("srt_caption_url")]
    public string? SrtUrl { get; set; }

    [Display("VTT URL")]
    [JsonProperty("vtt_caption_url")]
    public string? VttUrl { get; set; }

    [Display("Correlation ID")]
    [JsonProperty("callback_id")]
    public string? CallbackId { get; set; }

    [Display("Failure code")]
    [JsonProperty("failure_code")]
    public string? FailureCode { get; set; }

    [Display("Failure message")]
    [JsonProperty("failure_message")]
    public string? FailureMessage { get; set; }

    [Display("Created at")]
    [JsonProperty("created_at")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? CreatedAt { get; set; }
}
