using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class BatchCreateInput
{
    [Display("Submission key", Description = "Use a stable unique value for this job. Reuse it only to retry the same job within 24 hours.")]
    public string SubmissionKey { get; set; } = string.Empty;

    [Display("Batch type")]
    [StaticDataSource(typeof(BatchTypeHandler))]
    public string BatchType { get; set; } = string.Empty;

    [Display("Job inputs (JSON array)")]
    public string Items { get; set; } = string.Empty;

    [Display("Batch title")]
    public string? Title { get; set; }
}
