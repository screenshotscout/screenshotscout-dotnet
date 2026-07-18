using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using Xunit;

namespace ScreenshotScout.Tests;

public sealed class ScreenshotScoutClientTests
{
    [Fact]
    public void ConfigurationValidatesCredentials()
    {
        Assert.Throws<ScreenshotScoutConfigurationException>(
            () => new ScreenshotScoutClient(null!));
        Assert.Throws<ScreenshotScoutConfigurationException>(
            () => new ScreenshotScoutClient("   "));
        Assert.Throws<ScreenshotScoutConfigurationException>(
            () => new ScreenshotScoutClient("key with spaces"));
        Assert.Throws<ScreenshotScoutConfigurationException>(
            () => new ScreenshotScoutClient("key\n"));
        Assert.Throws<ScreenshotScoutConfigurationException>(
            () => new ScreenshotScoutClient(
                "key",
                string.Empty));
        Assert.Throws<ScreenshotScoutConfigurationException>(
            () => new ScreenshotScoutClient(
                "key",
                "   "));

        using var transport = new TestTransport();
        using var client = new ScreenshotScoutClient(
            "key",
            secretKey: null,
            options: new ScreenshotScoutClientOptions
            {
                HttpClient = transport.Client,
            });
    }

    [Fact]
    public async Task PostIsDefaultAndSerializesCompleteNullableOptionSurface()
    {
        using var transport = new TestTransport();
        using var client = CreateClient("access-key", transport);
        using var cancellationSource = new CancellationTokenSource();

        var options = new CaptureOptions
        {
            Format = new CaptureFormat("future-format"),
            ResponseType = new CaptureResponseType("future-response"),
            Country = string.Empty,
            Proxy = string.Empty,
            GeolocationLatitude = 0,
            GeolocationLongitude = 0,
            GeolocationAccuracy = 0,
            Cookies = ["session=a", "session=a", ""],
            Headers = ["X-Test:one", "X-Test:one", ""],
            Timeout = 0,
            WaitUntil = new CaptureWaitUntil("future-wait-state"),
            NavigationTimeout = 0,
            Delay = 0,
            Device = string.Empty,
            DeviceViewportWidth = 0,
            DeviceViewportHeight = 0,
            DeviceScaleFactor = 0,
            DeviceIsMobile = false,
            DeviceHasTouch = false,
            DeviceUserAgent = string.Empty,
            Timezone = string.Empty,
            MediaType = new CaptureMediaType("future-media"),
            ColorScheme = new CaptureColorScheme("future-scheme"),
            ReducedMotion = false,
            FullPage = false,
            FullPagePreScroll = false,
            FullPagePreScrollStep = 0,
            FullPagePreScrollStepDelay = 0,
            FullPageMaxHeight = 0,
            BlockCookieBanners = false,
            BlockAds = false,
            BlockChatWidgets = false,
            HideSelectors = [".same", ".same", ""],
            ClickSelectors = ["#first", "#first", ""],
            ClickAllSelectors = [".all", ".all", ""],
            InjectCss = ["", "body { color: red; }"],
            InjectJs = ["", "document.title = 'x';"],
            BypassCsp = false,
            Selector = string.Empty,
            ClipX = 0,
            ClipY = 0,
            ClipWidth = 0,
            ClipHeight = 0,
            ImageWidth = 0,
            ImageHeight = 0,
            ImageMode = new CaptureImageMode("future-image-mode"),
            ImageAnchor = new CaptureImageAnchor("future-anchor"),
            ImageAllowUpscale = false,
            ImageBackground = string.Empty,
            ImageQuality = 0,
            PdfPaperFormat = new CapturePdfPaperFormat("future-paper"),
            PdfLandscape = false,
            PdfPrintBackground = false,
            PdfMargin = string.Empty,
            PdfMarginTop = string.Empty,
            PdfMarginRight = string.Empty,
            PdfMarginBottom = string.Empty,
            PdfMarginLeft = string.Empty,
            PdfScale = 0,
            Cache = false,
            CacheTtl = 0,
            CacheKey = string.Empty,
            StorageMode = new CaptureStorageMode("future-storage"),
            StorageEndpoint = string.Empty,
            StorageBucket = string.Empty,
            StorageRegion = string.Empty,
            StorageObjectKey = string.Empty,
        };

        _ = await client.CaptureAsync(string.Empty, options, cancellationSource.Token);

        var request = Assert.Single(transport.Requests);
        Assert.Equal("POST", request.Method);
        Assert.Equal("https://api.screenshotscout.com/v1/capture", request.Url);
        Assert.Equal("Bearer access-key", request.Authorization);
        Assert.Equal("application/json", request.ContentType);

        var actual = JsonNode.Parse(request.Body);
        var expected = JsonNode.Parse(
            """
            {
              "url": "",
              "format": "future-format",
              "response_type": "future-response",
              "country": "",
              "proxy": "",
              "geolocation_latitude": 0,
              "geolocation_longitude": 0,
              "geolocation_accuracy": 0,
              "cookies": ["session=a", "session=a", ""],
              "headers": ["X-Test:one", "X-Test:one", ""],
              "timeout": 0,
              "wait_until": "future-wait-state",
              "navigation_timeout": 0,
              "delay": 0,
              "device": "",
              "device_viewport_width": 0,
              "device_viewport_height": 0,
              "device_scale_factor": 0,
              "device_is_mobile": false,
              "device_has_touch": false,
              "device_user_agent": "",
              "timezone": "",
              "media_type": "future-media",
              "color_scheme": "future-scheme",
              "reduced_motion": false,
              "full_page": false,
              "full_page_pre_scroll": false,
              "full_page_pre_scroll_step": 0,
              "full_page_pre_scroll_step_delay": 0,
              "full_page_max_height": 0,
              "block_cookie_banners": false,
              "block_ads": false,
              "block_chat_widgets": false,
              "hide_selectors": [".same", ".same", ""],
              "click_selectors": ["#first", "#first", ""],
              "click_all_selectors": [".all", ".all", ""],
              "inject_css": ["", "body { color: red; }"],
              "inject_js": ["", "document.title = 'x';"],
              "bypass_csp": false,
              "selector": "",
              "clip_x": 0,
              "clip_y": 0,
              "clip_width": 0,
              "clip_height": 0,
              "image_width": 0,
              "image_height": 0,
              "image_mode": "future-image-mode",
              "image_anchor": "future-anchor",
              "image_allow_upscale": false,
              "image_background": "",
              "image_quality": 0,
              "pdf_paper_format": "future-paper",
              "pdf_landscape": false,
              "pdf_print_background": false,
              "pdf_margin": "",
              "pdf_margin_top": "",
              "pdf_margin_right": "",
              "pdf_margin_bottom": "",
              "pdf_margin_left": "",
              "pdf_scale": 0,
              "cache": false,
              "cache_ttl": 0,
              "cache_key": "",
              "storage_mode": "future-storage",
              "storage_endpoint": "",
              "storage_bucket": "",
              "storage_region": "",
              "storage_object_key": ""
            }
            """);
        Assert.True(JsonNode.DeepEquals(expected, actual));
    }

    [Fact]
    public async Task NullOptionsAndEmptyRepeatedCollectionsAreOmittedWithoutDefaults()
    {
        using var transport = new TestTransport();
        using var client = CreateClient("key", transport);

        _ = await client.CaptureAsync(
            "https://example.com",
            new CaptureOptions
            {
                Format = null,
                Cookies = [],
                Headers = [],
                HideSelectors = [],
                ClickSelectors = [],
                ClickAllSelectors = [],
                InjectCss = [],
                InjectJs = [],
            },
            TestContext.Current.CancellationToken);

        var request = Assert.Single(transport.Requests);
        var body = JsonNode.Parse(request.Body);
        Assert.True(JsonNode.DeepEquals(
            JsonNode.Parse("""{"url":"https://example.com"}"""),
            body));
    }

    [Fact]
    public async Task GetUsesRepeatedQueryValuesAndBearerAuthentication()
    {
        using var transport = new TestTransport();
        using var client = CreateClient("query-secret", transport);

        _ = await client.CaptureAsync(
            "https://example.com/a path",
            new CaptureOptions
            {
                Cookies = ["a=1", "a=1"],
                Headers = ["X-Test:one", "X-Test:one"],
                Delay = 0,
                FullPage = false,
                CacheKey = string.Empty,
            },
            CaptureHttpMethod.Get,
            TestContext.Current.CancellationToken);

        var request = Assert.Single(transport.Requests);
        Assert.Equal("GET", request.Method);
        Assert.Equal("Bearer query-secret", request.Authorization);
        Assert.Null(request.ContentType);
        Assert.Empty(request.Body);
        Assert.Equal(
            "https://api.screenshotscout.com/v1/capture?url=https%3A%2F%2Fexample.com%2Fa+path&cookies=a%3D1&cookies=a%3D1&headers=X-Test%3Aone&headers=X-Test%3Aone&delay=0&full_page=false&cache_key=",
            request.Url);
        Assert.DoesNotContain("access_key=", request.Url, StringComparison.Ordinal);
        Assert.DoesNotContain("signature=", request.Url, StringComparison.Ordinal);
    }

    [Fact]
    public void BuildCaptureUrlIsPureAndIncludesOnlyTheNecessaryCredentialParameter()
    {
        using var transport = new TestTransport();
        using var client = CreateClient("query-key", transport);

        var url = client.BuildCaptureUrl(
            "https://example.com/a path",
            new CaptureOptions
            {
                Cookies = ["a=1", "a=1"],
                Headers = ["X-Test:one", "X-Test:one"],
                Delay = 0,
                FullPage = false,
                CacheKey = string.Empty,
            });

        Assert.Empty(transport.Requests);
        Assert.Equal(
            "https://api.screenshotscout.com/v1/capture?access_key=query-key&url=https%3A%2F%2Fexample.com%2Fa+path&cookies=a%3D1&cookies=a%3D1&headers=X-Test%3Aone&headers=X-Test%3Aone&delay=0&full_page=false&cache_key=",
            url);
    }

    [Fact]
    public async Task UnsafeValuesFailLocallyWithoutCallingTheTransport()
    {
        using var transport = new TestTransport();
        using var client = CreateClient("key", transport);

        await Assert.ThrowsAsync<ScreenshotScoutSerializationException>(
            () => client.CaptureAsync(
                null!,
                cancellationToken: TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ScreenshotScoutSerializationException>(
            () => client.CaptureAsync(
                "https://example.com",
                new CaptureOptions { PdfScale = double.PositiveInfinity },
                TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ScreenshotScoutSerializationException>(
            () => client.CaptureAsync(
                "https://example.com",
                new CaptureOptions { Headers = ["X-Test:one", null!] },
                TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ScreenshotScoutSerializationException>(
            () => client.CaptureAsync(
                "https://example.com",
                new CaptureOptions { Format = default(CaptureFormat) },
                TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ScreenshotScoutSerializationException>(
            () => client.CaptureAsync(
                "https://example.com",
                null,
                (CaptureHttpMethod)99,
                TestContext.Current.CancellationToken));

        Assert.Empty(transport.Requests);
    }

    [Fact]
    public void MethodIsClosedWhileServiceValuesUseSafeExplicitConversions()
    {
        Assert.Equal([CaptureHttpMethod.Post, CaptureHttpMethod.Get], Enum.GetValues<CaptureHttpMethod>());
        Assert.Null(typeof(CaptureOptions).GetProperty("Method"));

        var futureFormat = (CaptureFormat)"future-format";
        var options = new CaptureOptions { Format = (CaptureFormat)"future-format" };
        Assert.Equal("future-format", futureFormat.Value);
        Assert.Equal("future-format", options.Format?.Value);
        Assert.Equal("webp", CaptureFormat.Webp.Value);
        Assert.Equal("json", CaptureResponseType.Json.Value);

        Type[] openValueTypes =
        [
            typeof(CaptureFormat),
            typeof(CaptureResponseType),
            typeof(CaptureWaitUntil),
            typeof(CaptureMediaType),
            typeof(CaptureColorScheme),
            typeof(CaptureImageMode),
            typeof(CaptureImageAnchor),
            typeof(CapturePdfPaperFormat),
            typeof(CaptureStorageMode),
        ];
        foreach (var openValueType in openValueTypes)
        {
            Assert.DoesNotContain(
                openValueType.GetMethods(),
                method => method.Name == "op_Implicit"
                    && method.GetParameters() is [{ ParameterType: var parameterType }]
                    && parameterType == typeof(string));
            Assert.Contains(
                openValueType.GetMethods(),
                method => method.Name == "op_Explicit"
                    && method.ReturnType == openValueType
                    && method.GetParameters() is [{ ParameterType: var parameterType }]
                    && parameterType == typeof(string));
        }

        var includeFormat = false;
        var omittedOptions = new CaptureOptions
        {
            Format = includeFormat ? CaptureFormat.Png : null,
        };
        Assert.Null(omittedOptions.Format);

        using var client = new ScreenshotScoutClient("key");
        Assert.DoesNotContain("format=", client.BuildCaptureUrl("https://example.com", omittedOptions));
    }

    private static ScreenshotScoutClient CreateClient(string accessKey, TestTransport transport)
    {
        return new ScreenshotScoutClient(
            accessKey,
            options: new ScreenshotScoutClientOptions { HttpClient = transport.Client });
    }
}
