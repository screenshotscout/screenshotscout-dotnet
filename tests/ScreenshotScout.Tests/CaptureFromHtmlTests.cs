using System.Text;
using System.Text.Json;

using Xunit;

namespace ScreenshotScout.Tests;

public sealed class CaptureFromHtmlTests
{
    private const string InvoiceHtml =
        "<!doctype html>\n<html><head><title>Invoice #1042</title></head>"
        + "<body><h1>Invoice #1042 for Zoë & \"Café\"</h1></body></html>\n";

    [Fact]
    public async Task SendsTheHtmlFirstInAPostBodyExactlyAsGiven()
    {
        using var transport = new TestTransport();
        using var client = CreateClient(transport);
        const string html = "  " + InvoiceHtml + "  ";

        _ = await client.CaptureFromHtmlAsync(
            html,
            new CaptureOptions
            {
                Format = CaptureFormat.Pdf,
                PdfPrintBackground = true,
                Cookies = ["session=abc; Domain=example.com"],
            },
            TestContext.Current.CancellationToken);

        var request = Assert.Single(transport.Requests);
        Assert.Equal("POST", request.Method);
        Assert.Equal("https://api.screenshotscout.com/v1/capture", request.Url);
        Assert.Equal("Bearer key", request.Authorization);
        Assert.Equal("application/json", request.ContentType);

        using var body = JsonDocument.Parse(request.Body);
        Assert.Equal(
            ["html", "format", "cookies", "pdf_print_background"],
            body.RootElement.EnumerateObject().Select(static property => property.Name));
        Assert.Equal(html, body.RootElement.GetProperty("html").GetString());
        Assert.Equal("pdf", body.RootElement.GetProperty("format").GetString());
        Assert.Equal(
            "session=abc; Domain=example.com",
            Assert.Single(body.RootElement.GetProperty("cookies").EnumerateArray()).GetString());
        Assert.True(body.RootElement.GetProperty("pdf_print_background").GetBoolean());
    }

    [Fact]
    public async Task DecodesResponsesTheSameWayAsCaptureAsync()
    {
        using var transport = new TestTransport((_, _) => Task.FromResult(TestTransport.Response(
            200,
            "OK",
            "application/json",
            Encoding.UTF8.GetBytes("""{"screenshot_url":"https://cdn.example/invoice.pdf","cache_status":"miss"}"""))));
        using var client = CreateClient(transport);

        var response = Assert.IsType<JsonCaptureResponse>(await client.CaptureFromHtmlAsync(
            InvoiceHtml,
            new CaptureOptions
            {
                Format = CaptureFormat.Pdf,
                ResponseType = CaptureResponseType.Json,
            },
            TestContext.Current.CancellationToken));

        Assert.Equal("https://cdn.example/invoice.pdf", response.Result.ScreenshotUrl);
        Assert.Equal("miss", response.Result.CacheStatus);
        Assert.Equal(200, response.RawResponse.StatusCode);
    }

    [Fact]
    public async Task ClientWithSecretKeyRefusesWithoutCallingTheTransport()
    {
        using var transport = new TestTransport();
        using var client = new ScreenshotScoutClient(
            "ak_test",
            "sk_test",
            new ScreenshotScoutClientOptions
            {
                HttpClient = transport.Client,
            });

        var error = await Assert.ThrowsAsync<ScreenshotScoutConfigurationException>(
            () => client.CaptureFromHtmlAsync(
                InvoiceHtml,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(
            "`CaptureFromHtmlAsync` sends the HTML in a POST body, which can't be signed. This client has a secret key: use a client without a secret key and an API key that doesn't require signed requests.",
            error.Message);
        Assert.Empty(transport.Requests);
    }

    [Fact]
    public async Task RefusalNamesTheVariableTheSecretKeyCameFrom()
    {
        var environment = new TestEnvironment("ak_env", "sk_env");
        using var transport = new TestTransport();
        using var client = new ScreenshotScoutClient(
            null,
            null,
            new ScreenshotScoutClientOptions
            {
                HttpClient = transport.Client,
            },
            environment.GetEnvironmentVariable);

        var error = await Assert.ThrowsAsync<ScreenshotScoutConfigurationException>(
            () => client.CaptureFromHtmlAsync(
                InvoiceHtml,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(
            "`CaptureFromHtmlAsync` sends the HTML in a POST body, which can't be signed. This client has a secret key from SCREENSHOTSCOUT_SECRET_KEY: use a client without a secret key and an API key that doesn't require signed requests.",
            error.Message);
        Assert.Empty(transport.Requests);
    }

    [Fact]
    public async Task OnlyNullOrInvalidTextFailsLocally()
    {
        using var transport = new TestTransport();
        using var client = CreateClient(transport);

        var missing = await Assert.ThrowsAsync<ScreenshotScoutSerializationException>(
            () => client.CaptureFromHtmlAsync(
                null!,
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal("html", missing.Option);
        Assert.Equal("The HTML to capture must be a string.", missing.Message);

        foreach (var html in new[] { "<p>\uD83D</p>", "<p>\uDE00</p>", "<p>\uDE00\uD83D</p>" })
        {
            var invalid = await Assert.ThrowsAsync<ScreenshotScoutSerializationException>(
                () => client.CaptureFromHtmlAsync(
                    html,
                    cancellationToken: TestContext.Current.CancellationToken));
            Assert.Equal("html", invalid.Option);
            Assert.Equal("The HTML to capture must contain valid Unicode text.", invalid.Message);
        }

        Assert.Empty(transport.Requests);

        // Emptiness, like everything else about the document, is the API's to judge.
        _ = await client.CaptureFromHtmlAsync(
            string.Empty,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("""{"html":""}""", Encoding.UTF8.GetString(Assert.Single(transport.Requests).Body));
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
            () => client.CaptureFromHtmlAsync(InvoiceHtml, cancellationToken: preCanceled.Token));
        Assert.Empty(transport.Requests);

        using var inFlight = new CancellationTokenSource();
        var capture = client.CaptureFromHtmlAsync(InvoiceHtml, cancellationToken: inFlight.Token);
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
    public async Task BodyOverTheSizeLimitArrivesAsAnApiException()
    {
        const string body =
            """{"error_code":"request_too_large","error_message":"The request body must be at most 5 MB."}""";
        using var transport = new TestTransport((_, _) => Task.FromResult(TestTransport.Response(
            413,
            "Content Too Large",
            "application/json",
            Encoding.UTF8.GetBytes(body))));
        using var client = CreateClient(transport);

        var error = await Assert.ThrowsAsync<ScreenshotScoutApiException>(
            () => client.CaptureFromHtmlAsync(
                InvoiceHtml,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(413, error.StatusCode);
        Assert.Equal("request_too_large", error.ErrorCode);
        Assert.Equal("The request body must be at most 5 MB.", error.ErrorMessage);
        Assert.Equal("The request body must be at most 5 MB.", error.Message);
        Assert.Equal(body, Encoding.UTF8.GetString(error.RawResponse.Body));
        Assert.Single(transport.Requests);
    }

    [Fact]
    public void HtmlIsTheArgumentOfItsOwnPostOnlyMethodAndNotACaptureOption()
    {
        var method = Assert.Single(
            typeof(ScreenshotScoutClient).GetMethods(),
            candidate => candidate.Name.Contains("Html", StringComparison.Ordinal));

        Assert.Equal("CaptureFromHtmlAsync", method.Name);
        Assert.Equal(typeof(Task<CaptureResponse>), method.ReturnType);
        Assert.Equal(
            [typeof(string), typeof(CaptureOptions), typeof(CancellationToken)],
            method.GetParameters().Select(static parameter => parameter.ParameterType));
        Assert.Null(typeof(CaptureOptions).GetProperty("Html"));
        Assert.Null(typeof(CaptureOptions).GetProperty("Url"));
    }

    private static ScreenshotScoutClient CreateClient(TestTransport transport)
    {
        return new ScreenshotScoutClient(
            "key",
            options: new ScreenshotScoutClientOptions { HttpClient = transport.Client });
    }
}
