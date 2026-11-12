# Screenshot Scout .NET SDK

The official .NET SDK for the [Screenshot Scout](https://screenshotscout.com/) screenshot API.

## Requirements

- .NET 8 or later

## Installation

Install the package from NuGet:

```shell
dotnet add package ScreenshotScout
```

## Get your API credentials

[Sign up for Screenshot Scout](https://screenshotscout.com/auth/signup) or sign in, then open the [API Keys page](https://screenshotscout.com/app/api-keys) and copy the access key. You only need the secret key for [signed requests](#signed-requests).

Set the access key as the `SCREENSHOTSCOUT_ACCESS_KEY` environment variable, or [pass it in code](#pass-the-keys-yourself).

## Capture a screenshot

`CaptureAsync` performs asynchronous I/O and completes when Screenshot Scout returns the final response.

```csharp
using ScreenshotScout;

using var client = new ScreenshotScoutClient(); // Reads SCREENSHOTSCOUT_ACCESS_KEY

var response = await client.CaptureAsync(
    "https://example.com",
    new CaptureOptions { FullPage = true });

if (response is not BinaryCaptureResponse binary)
{
    throw new InvalidOperationException("Expected a binary response.");
}

await File.WriteAllBytesAsync("screenshot.png", binary.Bytes);
Console.WriteLine(binary.ScreenshotUrl);
```

POST is the default, or GET when the client has a secret key. An omitted `ResponseType`, or `CaptureResponseType.Binary`, returns a `BinaryCaptureResponse`.

## Request a JSON result

```csharp
var response = await client.CaptureAsync(
    "https://example.com",
    new CaptureOptions
    {
        ResponseType = CaptureResponseType.Json,
    });

if (response is JsonCaptureResponse json)
{
    Console.WriteLine(json.Result.ScreenshotUrl);
}
```

`CaptureResult.AdditionalFields` retains unrecognized JSON fields as `JsonElement` values.

## Use GET

POST is the default, or GET when the client has a secret key. To send a GET request, pass `CaptureHttpMethod.Get`:

```csharp
var response = await client.CaptureAsync(
    "https://example.com",
    new CaptureOptions { Format = CaptureFormat.Webp },
    CaptureHttpMethod.Get);
```

## Build a capture URL

`BuildCaptureUrl` creates a capture URL without making an HTTP request.

```csharp
var captureUrl = client.BuildCaptureUrl(
    "https://example.com",
    new CaptureOptions
    {
        FullPage = true,
        BlockAds = true,
    });

Console.WriteLine(captureUrl);
```

The generated URL contains the access key. When a secret key is configured, the SDK signs it automatically; otherwise it is unsigned. Treat generated URLs as sensitive. Before exposing one to browsers or users, configure a secret key and enable **Require signed requests** on the [API Keys page](https://screenshotscout.com/app/api-keys).

## Signed requests

Set the API key's secret key as the `SCREENSHOTSCOUT_SECRET_KEY` environment variable. The client then signs its capture requests and the capture URLs it builds, with no change to your code:

```csharp
using var client = new ScreenshotScoutClient(); // Reads SCREENSHOTSCOUT_ACCESS_KEY and SCREENSHOTSCOUT_SECRET_KEY

var response = await client.CaptureAsync(
    "https://example.com",
    new CaptureOptions { FullPage = true });
```

The secret key is used locally and is never transmitted. See the [signed requests guide](https://screenshotscout.com/docs/signed-requests) for details.

## Pass the keys yourself

To use keys from somewhere other than the environment, such as a secrets manager or per-customer settings, pass them to the client:

```csharp
using var client = new ScreenshotScoutClient(
    settings.AccessKey,
    settings.SecretKey); // The secret key is optional
```

This constructor reads neither environment variable, even when a key you pass is `null`.

## Capture options

The target URL is the required first argument. Configure the request with `CaptureOptions`:

- Output: `Format`, `ResponseType`
- Network and location: `Country`, `Proxy`, `GeolocationLatitude`, `GeolocationLongitude`, `GeolocationAccuracy`
- Cookies and webpage headers: `Cookies`, `Headers`
- Timing: `Timeout`, `WaitUntil`, `NavigationTimeout`, `Delay`
- Device emulation: `Device`, `DeviceViewportWidth`, `DeviceViewportHeight`, `DeviceScaleFactor`, `DeviceIsMobile`, `DeviceHasTouch`, `DeviceUserAgent`
- Page behavior: `Timezone`, `MediaType`, `ColorScheme`, `ReducedMotion`
- Full page: `FullPage`, `FullPagePreScroll`, `FullPagePreScrollStep`, `FullPagePreScrollStepDelay`, `FullPageMaxHeight`
- Blocking: `BlockCookieBanners`, `BlockAds`, `BlockChatWidgets`
- DOM changes: `HideSelectors`, `ClickSelectors`, `ClickAllSelectors`, `InjectCss`, `InjectJs`, `BypassCsp`
- Framing: `Selector`, `ClipX`, `ClipY`, `ClipWidth`, `ClipHeight`
- Image output: `ImageWidth`, `ImageHeight`, `ImageMode`, `ImageAnchor`, `ImageAllowUpscale`, `ImageBackground`, `ImageQuality`
- PDF: `PdfPaperFormat`, `PdfLandscape`, `PdfPrintBackground`, `PdfMargin`, `PdfMarginTop`, `PdfMarginRight`, `PdfMarginBottom`, `PdfMarginLeft`, `PdfScale`, `PdfPageRanges`
- Caching: `Cache`, `CacheTtl`, `CacheKey`
- Storage: `StorageMode`, `StorageEndpoint`, `StorageBucket`, `StorageRegion`, `StorageObjectKey`

Use the provided constants for documented service values. For a newer or custom service value, construct the open value type explicitly:

```csharp
var documented = CaptureFormat.Webp;
var futureValue = new CaptureFormat("future-format");
```

The same pattern applies to `CaptureResponseType`, `CaptureWaitUntil`, `CaptureMediaType`, `CaptureColorScheme`, `CaptureImageMode`, `CaptureImageAnchor`, `CapturePdfPaperFormat`, and `CaptureStorageMode`.

Null properties and empty repeated collections are omitted. Empty strings, `false`, and zero are sent as specified. Repeated collections preserve their order and duplicates. Screenshot Scout reports invalid values or option combinations. See the [screenshot option reference](https://screenshotscout.com/docs/screenshot-options).

## Timeouts and cancellation

Service timing and caller cancellation are independent:

```csharp
using var cancellationSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));

var response = await client.CaptureAsync(
    "https://example.com",
    new CaptureOptions
    {
        Timeout = 180, // Screenshot Scout capture budget, in seconds
    },
    cancellationSource.Token); // how long this caller is willing to wait
```

`Timeout` controls the server-side capture budget. The `CancellationToken` parameter is optional; pass one when your application needs its own deadline or cancellation behavior. Cancellation throws `OperationCanceledException`.

## Custom HttpClient and ownership

Inject a reusable `HttpClient` when the application needs custom handlers, proxy configuration, or a transport timeout:

```csharp
using var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromMinutes(5),
};
using var client = new ScreenshotScoutClient(
    new ScreenshotScoutClientOptions { HttpClient = httpClient }); // Reads SCREENSHOTSCOUT_ACCESS_KEY
```

An injected `HttpClient` remains caller-owned. `ScreenshotScoutClient` does not modify or dispose it, so its timeout, proxy, and handler settings stay in effect.

## Raw responses and exceptions

Every successful response contains `RawResponse`, including the status code, reason phrase, headers, content type, and body bytes. `ScreenshotScoutApiException` and `ScreenshotScoutResponseDecodingException` include the same raw response.

The following exception types derive from `ScreenshotScoutException`:

- `ScreenshotScoutConfigurationException`
- `ScreenshotScoutSerializationException`
- `ScreenshotScoutTransportException`
- `ScreenshotScoutApiException`
- `ScreenshotScoutResponseDecodingException`

```csharp
try
{
    await client.CaptureAsync("https://example.com");
}
catch (ScreenshotScoutApiException exception)
{
    Console.Error.WriteLine($"{exception.StatusCode} {exception.ErrorCode}");
    Console.Error.WriteLine(exception.ErrorMessage);
    Console.Error.WriteLine(exception.Errors);
    Console.Error.WriteLine(exception.ResponseBody);
    Console.Error.WriteLine(exception.RawResponse.ContentType);
}
catch (ScreenshotScoutTransportException exception)
{
    Console.Error.WriteLine(exception.InnerException);
}
```

API exceptions expose parsed `ErrorCode`, `ErrorMessage`, `Errors`, and the complete decoded `ResponseBody` when the failure body is valid JSON. For transport exceptions, `InnerException` contains the original .NET network failure.

## Examples

See [examples/README.md](examples/README.md) for standalone binary-capture, JSON-capture, and capture-URL examples.

## License

Licensed under the [MIT License](LICENSE).
