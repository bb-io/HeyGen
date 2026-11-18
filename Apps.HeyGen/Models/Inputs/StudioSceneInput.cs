using Apps.HeyGen.Handlers;
using Apps.HeyGen.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.HeyGen.Models.Inputs;

public class StudioSceneInput
{
    [Display("Scene type")]
    [StaticDataSource(typeof(StudioSceneTypeHandler))]
    public string Type { get; set; } = string.Empty;

    [Display("Avatar look ID", Description = "Required for a speaking avatar scene.")]
    public string? AvatarId { get; set; }

    [Display("Media file", Description = "Use a Blackbird image or video file for a still-image or video-clip scene.")]
    public FileReference? MediaFile { get; set; }

    [Display("Media URL")]
    public string? MediaUrl { get; set; }

    [Display("Media asset ID")]
    public string? MediaAssetId { get; set; }

    [Display("Script", Description = "Use script or recorded narration as the voiceover.")]
    public string? Script { get; set; }

    [Display("Voice ID", Description = "Required for scripted still images and video clips. Avatar scenes can use their default voice.")]
    [DataSource(typeof(VoiceHandler))]
    public string? VoiceId { get; set; }

    [Display("Narration file")]
    public FileReference? AudioFile { get; set; }

    [Display("Narration URL")]
    public string? AudioUrl { get; set; }

    [Display("Narration asset ID")]
    public string? AudioAssetId { get; set; }

    [Display("Still-image duration (seconds)", Description = "Required for a silent still image. Omit when the image has narration.")]
    public double? Duration { get; set; }

    [Display("Mute original clip audio", Description = "Applies to video-clip scenes.")]
    public bool? MuteClip { get; set; }
}
