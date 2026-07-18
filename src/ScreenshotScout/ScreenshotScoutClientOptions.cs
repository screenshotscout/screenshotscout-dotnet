namespace ScreenshotScout;

/// <summary>Configures the HTTP transport for a reusable client.</summary>
public sealed class ScreenshotScoutClientOptions
{
    /// <summary>
    /// Gets the optional caller-owned HTTP client. Its timeout and handler behavior
    /// are retained, and it is not disposed by <see cref="ScreenshotScoutClient"/>.
    /// </summary>
    public HttpClient? HttpClient { get; init; }
}
