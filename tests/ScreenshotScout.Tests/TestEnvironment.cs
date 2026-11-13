using System.Runtime.CompilerServices;

namespace ScreenshotScout.Tests;

// Stands in for the process environment, so tests never change real variables while classes run in parallel.
internal sealed class TestEnvironment
{
    private readonly Dictionary<string, string> _variables = new(StringComparer.Ordinal);
    private readonly List<string> _readNames = [];

    // A null value leaves that variable unset.
    internal TestEnvironment(string? accessKey = null, string? secretKey = null, string? baseUrl = null)
    {
        if (accessKey is not null)
        {
            _variables["SCREENSHOTSCOUT_ACCESS_KEY"] = accessKey;
        }

        if (secretKey is not null)
        {
            _variables["SCREENSHOTSCOUT_SECRET_KEY"] = secretKey;
        }

        if (baseUrl is not null)
        {
            _variables["SCREENSHOTSCOUT_BASE_URL"] = baseUrl;
        }
    }

    internal IReadOnlyList<string> ReadNames => _readNames;

    // Tests that use the public constructors read the real environment, where SCREENSHOTSCOUT_BASE_URL is read
    // even when keys are passed. Clearing it once, before any test runs, keeps a base URL set on this machine
    // from sending their requests elsewhere.
    [ModuleInitializer]
    internal static void ClearProcessBaseUrl()
    {
        Environment.SetEnvironmentVariable("SCREENSHOTSCOUT_BASE_URL", null);
    }

    internal string? GetEnvironmentVariable(string name)
    {
        _readNames.Add(name);
        return _variables.GetValueOrDefault(name);
    }
}
