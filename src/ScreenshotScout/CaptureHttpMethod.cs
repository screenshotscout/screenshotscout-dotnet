namespace ScreenshotScout;

/// <summary>Identifies the HTTP method used for one capture request.</summary>
public enum CaptureHttpMethod
{
    /// <summary>Send capture options as a JSON POST body.</summary>
    Post = 0,

    /// <summary>Send capture options as GET query parameters.</summary>
    Get = 1,
}
