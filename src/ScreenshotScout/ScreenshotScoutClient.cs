using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ScreenshotScout;

/// <summary>A reusable client for the inline Screenshot Scout capture endpoint.</summary>
public sealed class ScreenshotScoutClient : IDisposable
{
    private const string CaptureEndpoint = "https://api.screenshotscout.com/v1/capture";
    private static readonly Regex AccessKeyPattern = new(
        "^[A-Za-z0-9\\-._~+/]+=*\\z",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly string _accessKey;
    private readonly string? _secretKey;
    private readonly AuthenticationHeaderValue _authorizationHeader;
    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;
    private int _disposed;

    /// <summary>Creates a reusable client with explicit credentials and optional transport configuration.</summary>
    public ScreenshotScoutClient(
        string accessKey,
        string? secretKey = null,
        ScreenshotScoutClientOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(accessKey))
        {
            throw new ScreenshotScoutConfigurationException(
                "A non-blank Screenshot Scout access key is required.");
        }

        if (!AccessKeyPattern.IsMatch(accessKey))
        {
            throw new ScreenshotScoutConfigurationException(
                "The Screenshot Scout access key is not a valid Bearer credential.");
        }

        AuthenticationHeaderValue authorizationHeader;
        try
        {
            authorizationHeader = new AuthenticationHeaderValue("Bearer", accessKey);
        }
        catch (FormatException exception)
        {
            throw new ScreenshotScoutConfigurationException(
                "The Screenshot Scout access key cannot be used in an Authorization header.",
                exception);
        }

        if (secretKey is not null)
        {
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new ScreenshotScoutConfigurationException(
                    "The Screenshot Scout secret key must be a non-blank string when provided.");
            }

            if (!WireFormatting.IsValidUnicode(secretKey))
            {
                throw new ScreenshotScoutConfigurationException(
                    "The Screenshot Scout secret key must contain valid Unicode text.");
            }
        }

        _accessKey = accessKey;
        _secretKey = secretKey;
        _authorizationHeader = authorizationHeader;
        if (options?.HttpClient is { } injectedHttpClient)
        {
            _httpClient = injectedHttpClient;
            _ownsHttpClient = false;
        }
        else
        {
            _httpClient = CreateDefaultHttpClient();
            _ownsHttpClient = true;
        }
    }

    /// <summary>
    /// Sends one POST capture request using Task-based asynchronous I/O and waits
    /// for the final inline Screenshot Scout response.
    /// </summary>
    public Task<CaptureResponse> CaptureAsync(
        string url,
        CaptureOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return CaptureAsync(url, options, CaptureHttpMethod.Post, cancellationToken);
    }

    /// <summary>
    /// Sends one GET or POST capture request using Task-based asynchronous I/O
    /// and waits for the final inline Screenshot Scout response.
    /// </summary>
    public async Task<CaptureResponse> CaptureAsync(
        string url,
        CaptureOptions? options,
        CaptureHttpMethod method,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        var serialized = CaptureSerializer.Serialize(url, options);
        ValidateMethod(method);
        var expectedKind = GetExpectedResponseKind(options);
        cancellationToken.ThrowIfCancellationRequested();
        var signature = CreateSignature(serialized.Pairs);

        using var request = CreateRequest(serialized, method, signature);
        request.Headers.Authorization = _authorizationHeader;
        HttpResponseMessage? response = null;
        RawResponse rawResponse;
        try
        {
            response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken).ConfigureAwait(false);
            var body = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            rawResponse = CreateRawResponse(response, body);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new ScreenshotScoutTransportException(
                "Screenshot Scout request failed before a complete HTTP response body was received.",
                exception);
        }
        finally
        {
            response?.Dispose();
        }

        if (rawResponse.StatusCode is < 200 or > 299)
        {
            throw CreateApiException(rawResponse);
        }

        return DecodeSuccessResponse(rawResponse, expectedKind);
    }

    /// <summary>
    /// Builds a sensitive GET capture URL without performing any network I/O.
    /// A configured secret key signs the URL automatically.
    /// </summary>
    public string BuildCaptureUrl(string url, CaptureOptions? options = null)
    {
        ThrowIfDisposed();
        var serialized = CaptureSerializer.Serialize(url, options);
        var signature = CreateSignature(serialized.Pairs);
        var pairs = new List<WirePair>(serialized.Pairs.Count + 2)
        {
            new("access_key", _accessKey),
        };
        pairs.AddRange(serialized.Pairs);
        if (signature is not null)
        {
            pairs.Add(new WirePair("signature", signature));
        }

        return CaptureEndpoint + "?" + CaptureSerializer.EncodeQuery(pairs);
    }

    /// <summary>Releases only transport resources created by this SDK client.</summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0 && _ownsHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    private static HttpClient CreateDefaultHttpClient()
    {
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = false,
            UseCookies = false,
        };
        return new HttpClient(handler, disposeHandler: true)
        {
            Timeout = Timeout.InfiniteTimeSpan,
        };
    }

    private static void ValidateMethod(CaptureHttpMethod method)
    {
        if (method is not CaptureHttpMethod.Get and not CaptureHttpMethod.Post)
        {
            throw new ScreenshotScoutSerializationException(
                "The capture method must be CaptureHttpMethod.Get or CaptureHttpMethod.Post.",
                "method");
        }
    }

    private static HttpRequestMessage CreateRequest(
        SerializedCaptureOptions serialized,
        CaptureHttpMethod method,
        string? signature)
    {
        try
        {
            string requestUrl;
            HttpContent? content = null;
            if (method == CaptureHttpMethod.Get)
            {
                var pairs = new List<WirePair>(serialized.Pairs);
                if (signature is not null)
                {
                    pairs.Add(new WirePair("signature", signature));
                }

                requestUrl = CaptureEndpoint + "?" + CaptureSerializer.EncodeQuery(pairs);
            }
            else
            {
                requestUrl = CaptureEndpoint;
                var body = new Dictionary<string, object>(serialized.Body, StringComparer.Ordinal);
                if (signature is not null)
                {
                    body["signature"] = signature;
                }

                content = new ByteArrayContent(CaptureSerializer.SerializeJsonBody(body));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            var request = new HttpRequestMessage(
                method == CaptureHttpMethod.Get ? HttpMethod.Get : HttpMethod.Post,
                requestUrl)
            {
                Content = content,
            };
            return request;
        }
        catch (ScreenshotScoutException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new ScreenshotScoutSerializationException(
                "The Screenshot Scout HTTP request could not be constructed.",
                innerException: exception);
        }
    }

    private string? CreateSignature(IReadOnlyList<WirePair> pairs)
    {
        if (_secretKey is null)
        {
            return null;
        }

        try
        {
            var canonicalQuery = CaptureSerializer.BuildCanonicalQuery(pairs, _accessKey);
            var hash = HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(_secretKey),
                Encoding.UTF8.GetBytes(canonicalQuery));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
        catch (Exception exception)
        {
            throw new ScreenshotScoutSerializationException(
                "The capture request could not be signed.",
                innerException: exception);
        }
    }

    private static RawResponse CreateRawResponse(HttpResponseMessage response, byte[] body)
    {
        var mutableHeaders = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        AddHeaders(mutableHeaders, response.Headers);
        AddHeaders(mutableHeaders, response.Content.Headers);

        var headers = new Dictionary<string, IReadOnlyList<string>>(
            mutableHeaders.Count,
            StringComparer.OrdinalIgnoreCase);
        foreach (var (name, values) in mutableHeaders)
        {
            headers[name] = Array.AsReadOnly(values.ToArray());
        }

        headers.TryGetValue("Content-Type", out var contentTypes);
        return new RawResponse(
            (int)response.StatusCode,
            response.ReasonPhrase,
            new ReadOnlyDictionary<string, IReadOnlyList<string>>(headers),
            contentTypes?.FirstOrDefault(),
            body);
    }

    private static void AddHeaders(
        Dictionary<string, List<string>> target,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> source)
    {
        foreach (var (name, values) in source)
        {
            if (!target.TryGetValue(name, out var collected))
            {
                collected = [];
                target[name] = collected;
            }

            collected.AddRange(values);
        }
    }

    private static ScreenshotScoutApiException CreateApiException(RawResponse rawResponse)
    {
        var responseBody = TryParseJson(rawResponse.Body);
        string? errorCode = null;
        string? errorMessage = null;
        IReadOnlyList<JsonElement>? errors = null;
        if (responseBody is { ValueKind: JsonValueKind.Object } objectBody)
        {
            if (objectBody.TryGetProperty("error_code", out var code)
                && code.ValueKind == JsonValueKind.String)
            {
                errorCode = code.GetString();
            }

            if (objectBody.TryGetProperty("error_message", out var message)
                && message.ValueKind == JsonValueKind.String)
            {
                errorMessage = message.GetString();
            }

            if (objectBody.TryGetProperty("errors", out var errorValues)
                && errorValues.ValueKind == JsonValueKind.Array)
            {
                errors = Array.AsReadOnly(errorValues.EnumerateArray().Select(static value => value.Clone()).ToArray());
            }
        }

        return new ScreenshotScoutApiException(
            rawResponse,
            responseBody,
            errorCode,
            errorMessage,
            errors);
    }

    private static CaptureResponse DecodeSuccessResponse(
        RawResponse rawResponse,
        ExpectedResponseKind? expectedKind)
    {
        var actualKind = IsJsonMediaType(rawResponse.ContentType)
            ? ExpectedResponseKind.Json
            : ExpectedResponseKind.Binary;
        if (expectedKind is not null && actualKind != expectedKind)
        {
            var actual = actualKind == ExpectedResponseKind.Json ? "json" : "binary";
            var expected = expectedKind == ExpectedResponseKind.Json ? "json" : "binary";
            throw new ScreenshotScoutResponseDecodingException(
                $"Screenshot Scout returned a successful {actual} response when {expected} was requested.",
                rawResponse,
                new InvalidDataException($"Expected a {expected} response but received {actual}."));
        }

        return actualKind == ExpectedResponseKind.Json
            ? DecodeJsonResponse(rawResponse)
            : DecodeBinaryResponse(rawResponse);
    }

    private static BinaryCaptureResponse DecodeBinaryResponse(RawResponse rawResponse)
    {
        return new BinaryCaptureResponse(
            rawResponse.Body,
            FirstHeader(rawResponse.Headers, "Screenshot-Scout-Screenshot-URL"),
            FirstHeader(rawResponse.Headers, "Screenshot-Scout-Screenshot-URL-Expires-At"),
            FirstHeader(rawResponse.Headers, "Screenshot-Scout-Cache-Status"),
            rawResponse);
    }

    private static JsonCaptureResponse DecodeJsonResponse(RawResponse rawResponse)
    {
        JsonElement root;
        try
        {
            using var document = JsonDocument.Parse(rawResponse.Body);
            root = document.RootElement.Clone();
        }
        catch (JsonException exception)
        {
            throw new ScreenshotScoutResponseDecodingException(
                "Screenshot Scout returned a successful JSON response that could not be decoded.",
                rawResponse,
                exception);
        }

        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new ScreenshotScoutResponseDecodingException(
                "Screenshot Scout returned a successful JSON response that was not an object.",
                rawResponse,
                new JsonException("Expected a JSON object."));
        }

        var screenshotUrl = ReadOptionalString(root, "screenshot_url", rawResponse);
        var screenshotUrlExpiresAt = ReadOptionalString(root, "screenshot_url_expires_at", rawResponse);
        var cacheStatus = ReadOptionalString(root, "cache_status", rawResponse);
        var additionalFields = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
        foreach (var property in root.EnumerateObject())
        {
            if (property.Name is "screenshot_url" or "screenshot_url_expires_at" or "cache_status")
            {
                continue;
            }

            additionalFields[property.Name] = property.Value.Clone();
        }

        var result = new CaptureResult(
            screenshotUrl,
            screenshotUrlExpiresAt,
            cacheStatus,
            CaptureResult.ReadOnlyFields(additionalFields));
        return new JsonCaptureResponse(result, rawResponse);
    }

    private static string? ReadOptionalString(
        JsonElement value,
        string propertyName,
        RawResponse rawResponse)
    {
        if (!value.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        if (property.ValueKind != JsonValueKind.String)
        {
            throw new ScreenshotScoutResponseDecodingException(
                $"Screenshot Scout returned a non-string \"{propertyName}\" JSON field.",
                rawResponse,
                new JsonException($"Expected \"{propertyName}\" to be a string."));
        }

        return property.GetString();
    }

    private static JsonElement? TryParseJson(byte[] body)
    {
        try
        {
            using var document = JsonDocument.Parse(body);
            return document.RootElement.Clone();
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? FirstHeader(
        IReadOnlyDictionary<string, IReadOnlyList<string>> headers,
        string name)
    {
        return headers.TryGetValue(name, out var values) ? values.FirstOrDefault() : null;
    }

    private static bool IsJsonMediaType(string? contentType)
    {
        if (contentType is null)
        {
            return false;
        }

        var separator = contentType.IndexOf(';');
        var mediaType = (separator < 0 ? contentType : contentType[..separator]).Trim();
        return mediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase)
            || mediaType.EndsWith("+json", StringComparison.OrdinalIgnoreCase);
    }

    private static ExpectedResponseKind? GetExpectedResponseKind(CaptureOptions? options)
    {
        if (options?.ResponseType is not { } responseType
            || responseType.Value == CaptureResponseType.Binary.Value)
        {
            return ExpectedResponseKind.Binary;
        }

        return responseType.Value == CaptureResponseType.Json.Value
            ? ExpectedResponseKind.Json
            : null;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
    }

    private enum ExpectedResponseKind
    {
        Binary,
        Json,
    }
}
