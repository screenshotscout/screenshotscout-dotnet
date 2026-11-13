namespace ScreenshotScout;

/// <summary>Configures the base URL and the HTTP transport for a reusable client.</summary>
public sealed class ScreenshotScoutClientOptions
{
    /// <summary>
    /// Gets the optional base URL of another server to send requests to, such as your own proxy or a mock server.
    /// Captures go to this URL plus <c>/v1/capture</c>, and <see cref="ScreenshotScoutClient.BuildCaptureUrl"/>
    /// builds URLs on it. Defaults to <c>SCREENSHOTSCOUT_BASE_URL</c>, then to the Screenshot Scout API.
    /// </summary>
    public string? BaseUrl { get; init; }

    /// <summary>
    /// Gets the optional caller-owned HTTP client. Its timeout and handler behavior
    /// are retained, and it is not disposed by <see cref="ScreenshotScoutClient"/>.
    /// </summary>
    public HttpClient? HttpClient { get; init; }
}
