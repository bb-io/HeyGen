using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.HeyGen.Models.Inputs;

public class XliffInput
{
    [Display("Translated XLIFF file")]
    public FileReference File { get; set; } = default!;
}
