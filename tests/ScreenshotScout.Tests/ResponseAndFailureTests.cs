using System.Text;
using System.Text.Json;

using Xunit;

namespace ScreenshotScout.Tests;

public sealed class ResponseAndFailureTests
{
    [Fact]
    public async Task GetPostAndGeneratedUrlSigningMatchTheApiAlgorithm()
    {
        using var transport = new TestTransport((_, _) => Task.FromResult(TestTransport.Response(
            200,
            "OK",
            "application/json",
            Encoding.UTF8.GetBytes("{}"))));
        using var client = new ScreenshotScoutClient(
            "ak_test",
            "sk_test",
            new ScreenshotScoutClientOptions
            {
                HttpClient = transport.Client,
            });
        const string targetUrl = "https://example.com/a b?x=1&y=2";
        var options = new CaptureOptions
        {
            ResponseType = CaptureResponseType.Json,
            FullPage = false,
            Delay = 0,
            Headers = ["X-Test:one", "X-Test:two"],
        };
        const string expectedSignature =
            "0c4928ba691575903f27b911b8ea1a536604ca070d60d886e10c127c05e236fc";

        _ = await client.CaptureAsync(
            targetUrl,
            options,
            CaptureHttpMethod.Get,
            TestContext.Current.CancellationToken);
        _ = await client.CaptureAsync(
            targetUrl,
            options,
            CaptureHttpMethod.Post,
            TestContext.Current.CancellationToken);
        var builtUrl = client.BuildCaptureUrl(targetUrl, options);

        Assert.Equal(2, transport.Requests.Count);
        Assert.Equal(expectedSignature, QueryValue(transport.Requests[0].Url, "signature"));
        using var postDocument = JsonDocument.Parse(transport.Requests[1].Body);
        Assert.Equal(expectedSignature, postDocument.RootElement.GetProperty("signature").GetString());
        Assert.Equal(expectedSignature, QueryValue(builtUrl, "signature"));
        Assert.Equal("ak_test", QueryValue(builtUrl, "access_key"));
        Assert.DoesNotContain("sk_test", builtUrl, StringComparison.Ordinal);
        Assert.DoesNotContain("access_key=", transport.Requests[0].Url, StringComparison.Ordinal);
        Assert.False(postDocument.RootElement.TryGetProperty("access_key", out _));
        Assert.All(transport.Requests, request => Assert.Equal("Bearer ak_test", request.Authorization));
    }

    [Fact]
    public async Task BinaryResponseRetainsBytesMetadataAndRawHttpData()
    {
        var headers = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Screenshot-Scout-Cache-Status"] = ["HIT"],
            ["Screenshot-Scout-Screenshot-URL"] = ["https://cdn.example/screenshot.png"],
            ["Screenshot-Scout-Screenshot-URL-Expires-At"] = ["2026-07-14T00:00:00Z"],
            ["Set-Cookie"] = ["a=1; Path=/", "b=2; Path=/"],
        };
        using var transport = new TestTransport((_, _) => Task.FromResult(TestTransport.Response(
            201,
            "Created",
            "image/png",
            [0, 1, 2, 255],
            headers)));
        using var client = CreateClient(transport);

        var response = Assert.IsType<BinaryCaptureResponse>(
            await client.CaptureAsync(
                "https://example.com",
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal([0, 1, 2, 255], response.Bytes);
        Assert.Equal("https://cdn.example/screenshot.png", response.ScreenshotUrl);
        Assert.Equal("2026-07-14T00:00:00Z", response.ScreenshotUrlExpiresAt);
        Assert.Equal("HIT", response.CacheStatus);
        Assert.Equal(201, response.RawResponse.StatusCode);
        Assert.Equal("Created", response.RawResponse.ReasonPhrase);
        Assert.Equal("image/png", response.RawResponse.ContentType);
        Assert.Equal([0, 1, 2, 255], response.RawResponse.Body);
        Assert.Equal(
            ["a=1; Path=/", "b=2; Path=/"],
            response.RawResponse.Headers["Set-Cookie"]);
    }

    [Fact]
    public async Task JsonResponseRetainsDocumentedAndAdditionalFields()
    {
        const string json =
            """
            {
              "screenshot_url": "https://cdn.example/screenshot.png",
              "cache_status": "miss",
              "request_id": "request-123",
              "future": { "nested": true }
            }
            """;
        using var transport = new TestTransport((_, _) => Task.FromResult(TestTransport.Response(
            200,
            "OK",
            "application/vnd.screenshotscout+json; charset=utf-8",
            Encoding.UTF8.GetBytes(json))));
        using var client = CreateClient(transport);

        var response = Assert.IsType<JsonCaptureResponse>(await client.CaptureAsync(
            "https://example.com",
            new CaptureOptions { ResponseType = CaptureResponseType.Json },
            TestContext.Current.CancellationToken));

        Assert.Equal("https://cdn.example/screenshot.png", response.Result.ScreenshotUrl);
        Assert.Null(response.Result.ScreenshotUrlExpiresAt);
        Assert.Equal("miss", response.Result.CacheStatus);
        Assert.Equal("request-123", response.Result.AdditionalFields["request_id"].GetString());
        Assert.True(response.Result.AdditionalFields["future"].GetProperty("nested").GetBoolean());
        Assert.Equal(json, Encoding.UTF8.GetString(response.RawResponse.Body));
    }

    [Fact]
    public async Task KnownResponseTypesRejectWrongSuccessfulMediaType()
    {
        using var jsonTransport = new TestTransport((_, _) => Task.FromResult(TestTransport.Response(
            200,
            "OK",
            "application/json",
            Encoding.UTF8.GetBytes("{\"screenshot_url\":\"https://cdn.example/a.png\"}"))));
        using var binaryClient = CreateClient(jsonTransport);

        var binaryError = await Assert.ThrowsAsync<ScreenshotScoutResponseDecodingException>(
            () => binaryClient.CaptureAsync(
                "https://example.com",
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal("application/json", binaryError.RawResponse.ContentType);

        using var binaryTransport = new TestTransport((_, _) => Task.FromResult(TestTransport.Response(
            200,
            "OK",
            "image/png",
            [1, 2, 3])));
        using var jsonClient = CreateClient(binaryTransport);

        var jsonError = await Assert.ThrowsAsync<ScreenshotScoutResponseDecodingException>(
            () => jsonClient.CaptureAsync(
                "https://example.com",
                new CaptureOptions { ResponseType = CaptureResponseType.Json },
                TestContext.Current.CancellationToken));
        Assert.Equal("image/png", jsonError.RawResponse.ContentType);
        Assert.Equal([1, 2, 3], jsonError.RawResponse.Body);
    }

    [Fact]
    public async Task OpenResponseTypeUsesTheReturnedMediaType()
    {
        using var transport = new TestTransport((_, _) => Task.FromResult(TestTransport.Response(
            200,
            "OK",
            "application/json",
            Encoding.UTF8.GetBytes("{\"screenshot_url\":\"https://cdn.example/a.png\"}"))));
        using var client = CreateClient(transport);

        var response = Assert.IsType<JsonCaptureResponse>(await client.CaptureAsync(
            "https://example.com",
            new CaptureOptions { ResponseType = new CaptureResponseType("future-response") },
            TestContext.Current.CancellationToken));

        Assert.Equal("https://cdn.example/a.png", response.Result.ScreenshotUrl);
    }

    [Theory]
    [InlineData("{")]
    [InlineData("[]")]
    [InlineData("{\"screenshot_url\":123}")]
    public async Task MalformedSuccessfulJsonIsADecodingFailureWithRawAccess(string body)
    {
        using var transport = new TestTransport((_, _) => Task.FromResult(TestTransport.Response(
            200,
            "OK",
            "application/json",
            Encoding.UTF8.GetBytes(body))));
        using var client = CreateClient(transport);

        var error = await Assert.ThrowsAsync<ScreenshotScoutResponseDecodingException>(
            () => client.CaptureAsync(
                "https://example.com",
                new CaptureOptions { ResponseType = CaptureResponseType.Json },
                TestContext.Current.CancellationToken));

        Assert.Equal(200, error.RawResponse.StatusCode);
        Assert.Equal(body, Encoding.UTF8.GetString(error.RawResponse.Body));
    }

    [Fact]
    public async Task ApiFailureRetainsParsedFieldsAdditionalFieldsAndRawResponse()
    {
        const string body =
            """
            {
              "error_code": "invalid_options",
              "error_message": "One or more options are invalid.",
              "errors": [{ "option": "format", "message": "Unsupported." }],
              "request_id": "request-456"
            }
            """;
        var headers = new Dictionary<string, IReadOnlyList<string>>
        {
            ["X-Request-Id"] = ["request-456"],
        };
        using var transport = new TestTransport((_, _) => Task.FromResult(TestTransport.Response(
            400,
            "Bad Request",
            "application/json; charset=utf-8",
            Encoding.UTF8.GetBytes(body),
            headers)));
        using var client = CreateClient(transport);

        var error = await Assert.ThrowsAsync<ScreenshotScoutApiException>(
            () => client.CaptureAsync(
                "not-semantically-validated",
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(400, error.StatusCode);
        Assert.Equal("invalid_options", error.ErrorCode);
        Assert.Equal("One or more options are invalid.", error.ErrorMessage);
        Assert.Equal("format", Assert.Single(error.Errors!).GetProperty("option").GetString());
        Assert.Equal("request-456", error.ResponseBody?.GetProperty("request_id").GetString());
        Assert.Equal("Bad Request", error.RawResponse.ReasonPhrase);
        Assert.Equal("request-456", error.RawResponse.Headers["X-Request-Id"][0]);
        Assert.Equal(body, Encoding.UTF8.GetString(error.RawResponse.Body));
    }

    [Fact]
    public async Task RedirectAndNonJsonApiFailuresAreNotRetried()
    {
        using var transport = new TestTransport((_, _) => Task.FromResult(TestTransport.Response(
            302,
            "Found",
            "text/plain",
            Encoding.UTF8.GetBytes("redirect"),
            new Dictionary<string, IReadOnlyList<string>>
            {
                ["Location"] = ["https://other.example/v1/capture"],
            })));
        using var client = CreateClient(transport);

        var error = await Assert.ThrowsAsync<ScreenshotScoutApiException>(
            () => client.CaptureAsync(
                "https://example.com",
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(302, error.StatusCode);
        Assert.Null(error.ErrorCode);
        Assert.Null(error.ResponseBody);
        Assert.Equal("redirect", Encoding.UTF8.GetString(error.RawResponse.Body));
        Assert.Single(transport.Requests);
    }

    [Fact]
    public async Task TransportFailureRetainsNativeCauseAndIsNotRetried()
    {
        var nativeCause = new HttpRequestException("socket closed");
        using var transport = new TestTransport((_, _) => Task.FromException<HttpResponseMessage>(nativeCause));
        using var client = CreateClient(transport);

        var error = await Assert.ThrowsAsync<ScreenshotScoutTransportException>(
            () => client.CaptureAsync(
                "https://example.com",
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Same(nativeCause, error.InnerException);
        Assert.Single(transport.Requests);
    }

    [Fact]
    public async Task CallerCancellationUsesNormalCancellationSemantics()
    {
        using var transport = new TestTransport(async (_, cancellationToken) =>
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            throw new InvalidOperationException("unreachable");
        });
        using var client = CreateClient(transport);

        using var preCanceled = new CancellationTokenSource();
        preCanceled.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.CaptureAsync("https://example.com", cancellationToken: preCanceled.Token));
        Assert.Empty(transport.Requests);

        using var inFlight = new CancellationTokenSource();
        var capture = client.CaptureAsync("https://example.com", cancellationToken: inFlight.Token);
        while (transport.Requests.Count == 0)
        {
            await Task.Yield();
        }

        inFlight.Cancel();
        var error = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => capture);
        Assert.IsNotType<ScreenshotScoutTransportException>(error);
        Assert.Single(transport.Requests);
    }

    [Fact]
    public async Task CaptureTaskWaitsForTheInlineHttpResponse()
    {
        var responseGate = new TaskCompletionSource<HttpResponseMessage>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        using var transport = new TestTransport((_, _) => responseGate.Task);
        using var client = CreateClient(transport);

        var capture = client.CaptureAsync(
            "https://example.com",
            cancellationToken: TestContext.Current.CancellationToken);
        while (transport.Requests.Count == 0)
        {
            await Task.Yield();
        }

        Assert.False(capture.IsCompleted);
        responseGate.SetResult(TestTransport.Response(200, "OK", "image/png", [1]));
        _ = Assert.IsType<BinaryCaptureResponse>(await capture);
    }

    [Fact]
    public async Task InjectedHttpClientKeepsCallerConfigurationAndOwnership()
    {
        using var transport = new TestTransport();
        transport.Client.Timeout = TimeSpan.FromMinutes(5);
        var client = CreateClient(transport);

        Assert.Equal(TimeSpan.FromMinutes(5), transport.Client.Timeout);
        client.Dispose();
        Assert.False(transport.HandlerDisposed);

        using var response = await transport.Client.GetAsync(
            "https://example.test",
            TestContext.Current.CancellationToken);
        Assert.Equal(200, (int)response.StatusCode);
    }

    [Fact]
    public void ClientExposesNoBlockingCaptureWrapper()
    {
        Assert.DoesNotContain(
            typeof(ScreenshotScoutClient).GetMethods(),
            method => method.Name == "Capture");
        Assert.Equal(
            2,
            typeof(ScreenshotScoutClient).GetMethods().Count(method => method.Name == "CaptureAsync"));
    }

    private static ScreenshotScoutClient CreateClient(TestTransport transport)
    {
        return new ScreenshotScoutClient(
            "key",
            options: new ScreenshotScoutClientOptions { HttpClient = transport.Client });
    }

    private static string? QueryValue(string url, string name)
    {
        var queryIndex = url.IndexOf('?');
        if (queryIndex < 0)
        {
            return null;
        }

        foreach (var pair in url[(queryIndex + 1)..].Split('&'))
        {
            var separator = pair.IndexOf('=');
            var encodedName = separator < 0 ? pair : pair[..separator];
            if (!Uri.UnescapeDataString(encodedName.Replace('+', ' ')).Equals(name, StringComparison.Ordinal))
            {
                continue;
            }

            var encodedValue = separator < 0 ? string.Empty : pair[(separator + 1)..];
            return Uri.UnescapeDataString(encodedValue.Replace('+', ' '));
        }

        return null;
    }
}
