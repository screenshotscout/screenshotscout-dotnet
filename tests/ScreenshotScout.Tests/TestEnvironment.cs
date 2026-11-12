namespace ScreenshotScout.Tests;

// Stands in for the process environment, so tests never change real variables while classes run in parallel.
internal sealed class TestEnvironment
{
    private readonly Dictionary<string, string> _variables = new(StringComparer.Ordinal);
    private readonly List<string> _readNames = [];

    // A null value leaves that variable unset.
    internal TestEnvironment(string? accessKey = null, string? secretKey = null)
    {
        if (accessKey is not null)
        {
            _variables["SCREENSHOTSCOUT_ACCESS_KEY"] = accessKey;
        }

        if (secretKey is not null)
        {
            _variables["SCREENSHOTSCOUT_SECRET_KEY"] = secretKey;
        }
    }

    internal IReadOnlyList<string> ReadNames => _readNames;

    internal string? GetEnvironmentVariable(string name)
    {
        _readNames.Add(name);
        return _variables.GetValueOrDefault(name);
    }
}
