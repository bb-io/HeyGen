using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Models.Inputs;

public class CreateVideoFromAudioInput : AvatarInput
{
    [Display("Submission key", Description = "Reuse the key only to retry the same render within 24 hours.")]
    public string SubmissionKey { get; set; } = string.Empty;

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

    [Display("Correlation ID")]
    public string? CallbackId { get; set; }
}
