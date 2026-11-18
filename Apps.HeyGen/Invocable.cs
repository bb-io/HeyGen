using Apps.HeyGen.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.HeyGen;

public class Invocable(InvocationContext invocationContext) : BaseInvocable(invocationContext)
{
    private HeyGenClient? client;

    protected HeyGenClient Client => client ??= new(InvocationContext.AuthenticationCredentialsProviders);
}
