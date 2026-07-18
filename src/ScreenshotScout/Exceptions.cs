using System.Text.Json;

namespace ScreenshotScout;

/// <summary>Base class for Screenshot Scout SDK failures.</summary>
public class ScreenshotScoutException : Exception
{
    /// <summary>Initializes an SDK exception with a message.</summary>
    /// <param name="message">The error message.</param>
    public ScreenshotScoutException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes an SDK exception with a message and underlying exception.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused this failure, when available.</param>
    public ScreenshotScoutException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>Reports invalid client configuration.</summary>
public sealed class ScreenshotScoutConfigurationException : ScreenshotScoutException
{
    /// <summary>Initializes a client-configuration exception with a message.</summary>
    /// <param name="message">The error message.</param>
    public ScreenshotScoutConfigurationException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a client-configuration exception with a message and underlying exception.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused this failure, when available.</param>
    public ScreenshotScoutConfigurationException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>Reports capture values that cannot be serialized safely.</summary>
public sealed class ScreenshotScoutSerializationException : ScreenshotScoutException
{
    /// <summary>Initializes a capture-serialization exception.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="option">The public option name associated with the failure, when available.</param>
    /// <param name="innerException">The exception that caused this failure, when available.</param>
    public ScreenshotScoutSerializationException(
        string message,
        string? option = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Option = option;
    }

    /// <summary>Gets the public option name associated with the failure, when available.</summary>
    public string? Option { get; }
}

/// <summary>Reports a failure before a complete HTTP response body was received.</summary>
public sealed class ScreenshotScoutTransportException : ScreenshotScoutException
{
    /// <summary>Initializes a transport exception with the original network failure.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The original .NET transport exception.</param>
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

    /// <summary>Gets the HTTP status code returned by Screenshot Scout.</summary>
    public int StatusCode { get; }

    /// <summary>Gets the parsed <c>error_code</c> value, when present and valid.</summary>
    public string? ErrorCode { get; }

    /// <summary>Gets the parsed <c>error_message</c> value, when present and valid.</summary>
    public string? ErrorMessage { get; }

    /// <summary>Gets the parsed <c>errors</c> array, when present and valid.</summary>
    public IReadOnlyList<JsonElement>? Errors { get; }

    /// <summary>Gets the complete decoded JSON failure body, when the body contains valid JSON.</summary>
    public JsonElement? ResponseBody { get; }

    /// <summary>Gets the exact buffered HTTP response.</summary>
    public RawResponse RawResponse { get; }
}

/// <summary>Reports a successful response that could not be decoded as requested.</summary>
public sealed class ScreenshotScoutResponseDecodingException : ScreenshotScoutException
{
    /// <summary>Initializes a successful-response decoding exception.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="rawResponse">The exact buffered HTTP response that could not be decoded.</param>
    /// <param name="innerException">The underlying decoding failure, when available.</param>
    public ScreenshotScoutResponseDecodingException(
        string message,
        RawResponse rawResponse,
        Exception? innerException = null)
        : base(message, innerException)
    {
        RawResponse = rawResponse;
    }

    /// <summary>Gets the exact buffered HTTP response that could not be decoded.</summary>
    public RawResponse RawResponse { get; }
}
