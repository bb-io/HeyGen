using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Apps.HeyGen.Models.Outputs;

public class ProofreadDetails
{
    [Display("Proofread session ID")]
    [JsonProperty("id")]
    public string ProofreadId { get; set; } = string.Empty;

    [Display("Status")]
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [Display("Title")]
    [JsonProperty("title")]
    public string? Title { get; set; }

    [Display("Source language")]
    [JsonProperty("input_language")]
    public string? InputLanguage { get; set; }

    [Display("Target language")]
    [JsonProperty("output_language")]
    public string? OutputLanguage { get; set; }

    [Display("Submitted for review")]
    [JsonProperty("submitted_for_review")]
    public bool? SubmittedForReview { get; set; }

    [Display("Created at")]
    [JsonProperty("created_at")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? CreatedAt { get; set; }

    [Display("Failure message")]
    [JsonProperty("failure_message")]
    public string? FailureMessage { get; set; }
}
