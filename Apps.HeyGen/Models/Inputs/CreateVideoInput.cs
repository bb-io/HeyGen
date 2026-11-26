using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.HeyGen.Models.Inputs;

public class CreateVideoInput : AvatarInput
{
    [Display("Submission key", Description = "Use a stable unique value for this job. Reuse it only to retry the same job within 24 hours.")]
    public string SubmissionKey { get; set; } = string.Empty;

    [Display("Script text")]
    public string Script { get; set; } = string.Empty;

    [Display("Voice ID")]
    [DataSource(typeof(VoiceHandler))]
    public string? VoiceId { get; set; }

    [Display("Avatar engine")]
    [DataSource(typeof(EngineHandler))]
    public string? Engine { get; set; }

    [Display("Title")]
    public string? Title { get; set; }

    [Display("Aspect ratio")]
    [StaticDataSource(typeof(AvatarAspectHandler))]
    public string? AspectRatio { get; set; }

    [Display("Resolution")]
    [StaticDataSource(typeof(ResolutionHandler))]
    public string? Resolution { get; set; }

    [Display("Burn captions into the video")]
    public bool? BurnCaptions { get; set; }

    [Display("Brand glossary ID")]
    [DataSource(typeof(GlossaryHandler))]
    public string? BrandGlossaryId { get; set; }

    [Display("Correlation ID")]
    public string? CallbackId { get; set; }
}
