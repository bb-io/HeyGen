using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HeyGen.Models.Inputs;

public class CreateStudioVideoInput
{
    [Display("Scenes", Description = "One to 50 scenes in playback order.")]
    public List<StudioSceneInput>? Scenes { get; set; }

    [Display("Scenes JSON", Description = "Alternative to Scenes for a JSON array containing scripts, URLs, and HeyGen IDs.")]
    public string? ScenesJson { get; set; }

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

    [Display("Submission key mode", Description = "Choose Automatic per Flight or supply a custom key.")]
    [StaticDataSource(typeof(SubmissionKeyModeHandler))]
    public string? SubmissionKeyMode { get; set; }

    [Display("Render slot", Description = "Automatic mode. Leave blank for one render per Flight. Use a distinct stable name for each render inside a Flight.")]
    public string? RenderSlot { get; set; }

    [Display("Submission key", Description = "Custom mode. Reuse only to retry the same ordered scene list and revision.")]
    public string? SubmissionKey { get; set; }
}
