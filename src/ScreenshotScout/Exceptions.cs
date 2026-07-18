using System.Text.Json;

namespace ScreenshotScout;

/// <summary>Base class for Screenshot Scout SDK failures.</summary>
public class ScreenshotScoutException : Exception
{
    public ScreenshotScoutException(string message)
        : base(message)
    {
    }

    public ScreenshotScoutException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>Reports invalid client configuration.</summary>
public sealed class ScreenshotScoutConfigurationException : ScreenshotScoutException
{
    public ScreenshotScoutConfigurationException(string message)
        : base(message)
    {
    }

    public ScreenshotScoutConfigurationException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>Reports capture values that cannot be serialized safely.</summary>
public sealed class ScreenshotScoutSerializationException : ScreenshotScoutException
{
    public ScreenshotScoutSerializationException(
        string message,
        string? option = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Option = option;
    }

    public string? Option { get; }
}

/// <summary>Reports a failure before a complete HTTP response body was received.</summary>
public sealed class ScreenshotScoutTransportException : ScreenshotScoutException
{
    public ScreenshotScoutTransportException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>Reports a non-successful Screenshot Scout HTTP response.</summary>
public sealed class ScreenshotScoutApiException : ScreenshotScoutException
{
    internal ScreenshotScoutApiException(
        RawResponse rawResponse,
        JsonElement? responseBody,
        string? errorCode,
        string? errorMessage,
        IReadOnlyList<JsonElement>? errors)
        : base(errorMessage ?? $"Screenshot Scout API request failed with status {rawResponse.StatusCode}.")
    {
        StatusCode = rawResponse.StatusCode;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Errors = errors;
        ResponseBody = responseBody;
        RawResponse = rawResponse;
    }

    public int StatusCode { get; }
    public string? ErrorCode { get; }
    public string? ErrorMessage { get; }
    public IReadOnlyList<JsonElement>? Errors { get; }
    public JsonElement? ResponseBody { get; }
    public RawResponse RawResponse { get; }
}

/// <summary>Reports a successful response that could not be decoded as requested.</summary>
public sealed class ScreenshotScoutResponseDecodingException : ScreenshotScoutException
{
    public ScreenshotScoutResponseDecodingException(
        string message,
        RawResponse rawResponse,
        Exception? innerException = null)
        : base(message, innerException)
    {
        RawResponse = rawResponse;
    }

    public RawResponse RawResponse { get; }
}
