using Apps.HeyGen.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Connections;

public class ConnectionValidator(InvocationContext invocationContext)
    : BaseInvocable(invocationContext), IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> credentials, CancellationToken cancellationToken)
    {
        try
        {
            using var client = new HeyGenClient(credentials);
            await client.ExecuteWithErrorHandling(new RestRequest("api_keys/self"));
            return new() { IsValid = true, Message = "Success" };
        }
        catch (Exception exception)
        {
            InvocationContext.Logger?.LogError($"HeyGen connection validation failed. {exception.Message}", []);
            return new() { IsValid = false, Message = exception.Message };
        }
    }
}
