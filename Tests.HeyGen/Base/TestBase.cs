using System.Text.Json;
using Apps.HeyGen.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.HeyGen.Base;

public abstract class TestBase
{
    private JsonElement fixtures;

    protected InvocationContext Context { get; private set; } = default!;
    protected LocalFiles Files { get; private set; } = default!;

    [TestInitialize]
    public void Configure()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        var root = File.Exists(path) ? JsonDocument.Parse(File.ReadAllText(path)).RootElement.Clone() : default;
        var key = Environment.GetEnvironmentVariable("HEYGEN_API_KEY");
        if (string.IsNullOrWhiteSpace(key) && root.ValueKind == JsonValueKind.Object
            && root.TryGetProperty("ConnectionDefinition", out var connection)
            && connection.TryGetProperty(CredsNames.ApiKey, out var configuredKey))
            key = configuredKey.GetString();

        if (string.IsNullOrWhiteSpace(key))
            Assert.Inconclusive("Provide a disposable HeyGen workspace's API key to run this online test.");

        if (root.ValueKind == JsonValueKind.Object)
            root.TryGetProperty("Fixtures", out fixtures);

        Context = new InvocationContext
        {
            AuthenticationCredentialsProviders = [new AuthenticationCredentialsProvider(CredsNames.ApiKey, key!)]
        };
        Files = new LocalFiles();
    }

    [TestCleanup]
    public void CleanUp() => Files?.Dispose();

    protected static string SubmissionKey() => Guid.NewGuid().ToString("N");

    protected static void RequireWrites()
    {
        if (Environment.GetEnvironmentVariable("HEYGEN_TEST_ALLOW_WRITES") != "1")
            Assert.Inconclusive("Set HEYGEN_TEST_ALLOW_WRITES=1 for a disposable, funded HeyGen workspace.");
    }

    protected string Fixture(string key)
    {
        if (fixtures.ValueKind == JsonValueKind.Object && fixtures.TryGetProperty(key, out var value)
            && !string.IsNullOrWhiteSpace(value.GetString()))
            return value.GetString()!;

        Assert.Inconclusive($"Set the disposable {key} fixture in appsettings.json.");
        return string.Empty;
    }

    protected FileReference FixtureFile(string key)
    {
        var path = Path.GetFullPath(Fixture(key));
        if (!File.Exists(path))
            Assert.Inconclusive($"The file for {key} does not exist.");

        return Files.FromPath(path);
    }
}
