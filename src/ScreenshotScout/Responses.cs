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

    public int StatusCode { get; }
    public string? ReasonPhrase { get; }
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers { get; }
    public string? ContentType { get; }
    public byte[] Body { get; }
}

/// <summary>Base type for successful Screenshot Scout capture responses.</summary>
public abstract class CaptureResponse
{
    private protected CaptureResponse(RawResponse rawResponse)
    {
        RawResponse = rawResponse;
    }

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

    public byte[] Bytes { get; }
    public string? ScreenshotUrl { get; }
    public string? ScreenshotUrlExpiresAt { get; }
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

    public string? ScreenshotUrl { get; }
    public string? ScreenshotUrlExpiresAt { get; }
    public string? CacheStatus { get; }
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

    public CaptureResult Result { get; }
}
