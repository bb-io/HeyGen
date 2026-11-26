using Apps.HeyGen.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.HeyGen.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups =>
    [
        new()
        {
            Name = "API key",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties =
            [
                new(CredsNames.ApiKey)
                {
                    DisplayName = "API key",
                    Description = "Create a scoped API key in the HeyGen API dashboard.",
                    Sensitive = true
                }
            ]
        }
    ];

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        if (values.TryGetValue(CredsNames.ApiKey, out var key))
            yield return new(CredsNames.ApiKey, key.Trim());
    }
}
