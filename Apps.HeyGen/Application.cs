using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.HeyGen;

public class Application : IApplication, ICategoryProvider
{
    public string Name => "HeyGen";

    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.Multimedia];
        set { }
    }

    public T GetInstance<T>() => throw new NotImplementedException();
}
