using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.HeyGen.Models.Outputs;

public class FileOutput
{
    [Display("File")]
    public FileReference File { get; set; } = default!;
}
