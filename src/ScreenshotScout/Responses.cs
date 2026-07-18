using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScreenshotScout;

/// <summary>Contains the exact buffered HTTP response retained by the SDK.</summary>
public sealed class RawResponse
{
    internal RawResponse(
        int statusCode,
        string? reasonPhrase,
        IReadOnlyDictionary<string, IReadOnlyList<string>> headers,
        string? contentType,
        byte[] body)
    {
        StatusCode = statusCode;
        ReasonPhrase = reasonPhrase;
        Headers = headers;
        ContentType = contentType;
        Body = body;
    }

    /// <summary>Gets the integer HTTP status code.</summary>
    public int StatusCode { get; }

    /// <summary>Gets the HTTP reason phrase, when supplied by the transport.</summary>
    public string? ReasonPhrase { get; }

    /// <summary>Gets the response headers with all values retained for each header name.</summary>
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers { get; }

    /// <summary>Gets the response content type, including any parameters, when present.</summary>
    public string? ContentType { get; }

    /// <summary>Gets the exact buffered response-body bytes.</summary>
    /// <remarks>
    /// Treat the returned array as read-only. For binary responses, the same buffered data is
    /// also exposed by <see cref="BinaryCaptureResponse.Bytes"/>.
    /// </remarks>
    public byte[] Body { get; }
}

/// <summary>Base type for successful Screenshot Scout capture responses.</summary>
public abstract class CaptureResponse
{
    private protected CaptureResponse(RawResponse rawResponse)
    {
        RawResponse = rawResponse;
    }

    /// <summary>Gets the exact buffered HTTP response.</summary>
    public RawResponse RawResponse { get; }
}

/// <summary>Contains a buffered binary screenshot or PDF response.</summary>
public sealed class BinaryCaptureResponse : CaptureResponse
{
    internal BinaryCaptureResponse(
        byte[] bytes,
        string? screenshotUrl,
        string? screenshotUrlExpiresAt,
        string? cacheStatus,
        RawResponse rawResponse)
        : base(rawResponse)
    {
        Bytes = bytes;
        ScreenshotUrl = screenshotUrl;
        ScreenshotUrlExpiresAt = screenshotUrlExpiresAt;
        CacheStatus = cacheStatus;
    }

    /// <summary>Gets the exact screenshot or PDF response-body bytes.</summary>
    /// <remarks>
    /// Treat the returned array as read-only. The same buffered data is also exposed by
    /// <see cref="CaptureResponse.RawResponse"/> through <see cref="RawResponse.Body"/>.
    /// </remarks>
    public byte[] Bytes { get; }

    /// <summary>Gets the screenshot URL returned in the response headers, when present.</summary>
    public string? ScreenshotUrl { get; }

    /// <summary>Gets the cached screenshot URL expiration value, when caching was requested.</summary>
    public string? ScreenshotUrlExpiresAt { get; }

    /// <summary>Gets the cache status returned by Screenshot Scout, when caching was requested.</summary>
    public string? CacheStatus { get; }
}

/// <summary>Contains documented JSON result fields and unrecognized fields.</summary>
public sealed class CaptureResult
{
    internal CaptureResult(
        string? screenshotUrl,
        string? screenshotUrlExpiresAt,
        string? cacheStatus,
        IReadOnlyDictionary<string, JsonElement> additionalFields)
    {
        ScreenshotUrl = screenshotUrl;
        ScreenshotUrlExpiresAt = screenshotUrlExpiresAt;
        CacheStatus = cacheStatus;
        AdditionalFields = additionalFields;
    }

    /// <summary>Gets the screenshot URL, when present.</summary>
    public string? ScreenshotUrl { get; }

    /// <summary>Gets the cached screenshot URL expiration value, when caching was requested.</summary>
    public string? ScreenshotUrlExpiresAt { get; }

    /// <summary>Gets the cache status, when caching was requested.</summary>
    public string? CacheStatus { get; }

    /// <summary>Gets unrecognized JSON result fields without transformation.</summary>
    public IReadOnlyDictionary<string, JsonElement> AdditionalFields { get; }

    internal static IReadOnlyDictionary<string, JsonElement> ReadOnlyFields(
        IDictionary<string, JsonElement> fields)
    {
        return new ReadOnlyDictionary<string, JsonElement>(fields);
    }
}

/// <summary>Contains a decoded JSON capture result.</summary>
public sealed class JsonCaptureResponse : CaptureResponse
{
    internal JsonCaptureResponse(CaptureResult result, RawResponse rawResponse)
        : base(rawResponse)
    {
        Result = result;
    }

    /// <summary>Gets the decoded JSON capture result.</summary>
    public CaptureResult Result { get; }
}
