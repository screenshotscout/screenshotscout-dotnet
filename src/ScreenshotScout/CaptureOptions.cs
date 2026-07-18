namespace ScreenshotScout;

/// <summary>
/// Configures a Screenshot Scout capture. The target URL, credentials, signature,
/// request method, and cancellation token are supplied separately.
/// </summary>
public sealed class CaptureOptions
{
    /// <summary>Gets or initializes the screenshot or PDF output format.</summary>
    public CaptureFormat? Format { get; init; }

    /// <summary>Gets or initializes whether the API returns a binary body or a JSON result.</summary>
    public CaptureResponseType? ResponseType { get; init; }

    /// <summary>Gets or initializes the two-letter proxy country code. A custom proxy takes precedence.</summary>
    public string? Country { get; init; }

    /// <summary>Gets or initializes the HTTP, HTTPS, or SOCKS5 proxy used to load the target page.</summary>
    public string? Proxy { get; init; }

    /// <summary>Gets or initializes the latitude exposed through the browser Geolocation API.</summary>
    public double? GeolocationLatitude { get; init; }

    /// <summary>Gets or initializes the longitude exposed through the browser Geolocation API.</summary>
    public double? GeolocationLongitude { get; init; }

    /// <summary>Gets or initializes the accuracy exposed through the browser Geolocation API.</summary>
    public double? GeolocationAccuracy { get; init; }

    /// <summary>
    /// Gets or initializes ordered <c>Set-Cookie</c>-style cookie lines to install before navigation.
    /// Order and duplicates are preserved.
    /// </summary>
    public IReadOnlyList<string>? Cookies { get; init; }

    /// <summary>
    /// Gets or initializes ordered <c>Name:Value</c> webpage request headers.
    /// Order and duplicates are preserved.
    /// </summary>
    public IReadOnlyList<string>? Headers { get; init; }

    /// <summary>Gets or initializes the service-side request-wide capture timeout, in seconds.</summary>
    public int? Timeout { get; init; }

    /// <summary>Gets or initializes the event or network-idle condition that completes navigation.</summary>
    public CaptureWaitUntil? WaitUntil { get; init; }

    /// <summary>Gets or initializes the page-navigation timeout, in seconds.</summary>
    public int? NavigationTimeout { get; init; }

    /// <summary>Gets or initializes the delay after navigation and before capture, in seconds.</summary>
    public int? Delay { get; init; }

    /// <summary>Gets or initializes the browser device preset name.</summary>
    public string? Device { get; init; }

    /// <summary>Gets or initializes the emulated viewport width, in pixels.</summary>
    public int? DeviceViewportWidth { get; init; }

    /// <summary>Gets or initializes the emulated viewport height, in pixels.</summary>
    public int? DeviceViewportHeight { get; init; }

    /// <summary>Gets or initializes the emulated device scale factor (DPR).</summary>
    public double? DeviceScaleFactor { get; init; }

    /// <summary>Gets or initializes whether the emulated browser behaves as a mobile device.</summary>
    public bool? DeviceIsMobile { get; init; }

    /// <summary>Gets or initializes whether the emulated device supports touch input.</summary>
    public bool? DeviceHasTouch { get; init; }

    /// <summary>Gets or initializes the browser user-agent string.</summary>
    public string? DeviceUserAgent { get; init; }

    /// <summary>Gets or initializes the IANA timezone identifier emulated in the browser.</summary>
    public string? Timezone { get; init; }

    /// <summary>Gets or initializes the CSS media type emulated for the page.</summary>
    public CaptureMediaType? MediaType { get; init; }

    /// <summary>Gets or initializes the preferred color scheme emulated for the page.</summary>
    public CaptureColorScheme? ColorScheme { get; init; }

    /// <summary>Gets or initializes whether the browser emulates the reduced-motion preference.</summary>
    public bool? ReducedMotion { get; init; }

    /// <summary>Gets or initializes whether to capture the entire scrollable page.</summary>
    public bool? FullPage { get; init; }

    /// <summary>Gets or initializes whether to scroll a full page before capture to trigger lazy-loaded content.</summary>
    public bool? FullPagePreScroll { get; init; }

    /// <summary>Gets or initializes the vertical pre-scroll step size, in pixels.</summary>
    public int? FullPagePreScrollStep { get; init; }

    /// <summary>Gets or initializes the delay between pre-scroll steps, in milliseconds.</summary>
    public int? FullPagePreScrollStepDelay { get; init; }

    /// <summary>Gets or initializes the maximum full-page capture height, in pixels. Zero means unlimited.</summary>
    public int? FullPageMaxHeight { get; init; }

    /// <summary>Gets or initializes whether Screenshot Scout attempts to hide cookie banners.</summary>
    public bool? BlockCookieBanners { get; init; }

    /// <summary>Gets or initializes whether Screenshot Scout attempts to block advertisements.</summary>
    public bool? BlockAds { get; init; }

    /// <summary>Gets or initializes whether Screenshot Scout attempts to hide chat widgets and overlays.</summary>
    public bool? BlockChatWidgets { get; init; }

    /// <summary>
    /// Gets or initializes ordered CSS selectors whose matching elements are hidden.
    /// Order and duplicates are preserved.
    /// </summary>
    public IReadOnlyList<string>? HideSelectors { get; init; }

    /// <summary>
    /// Gets or initializes ordered CSS selectors whose first matching elements are clicked.
    /// Order and duplicates are preserved.
    /// </summary>
    public IReadOnlyList<string>? ClickSelectors { get; init; }

    /// <summary>
    /// Gets or initializes ordered CSS selectors whose matching elements are all clicked.
    /// Order and duplicates are preserved.
    /// </summary>
    public IReadOnlyList<string>? ClickAllSelectors { get; init; }

    /// <summary>
    /// Gets or initializes ordered CSS snippets injected before capture.
    /// Order and duplicates are preserved.
    /// </summary>
    public IReadOnlyList<string>? InjectCss { get; init; }

    /// <summary>
    /// Gets or initializes ordered JavaScript snippets injected before capture.
    /// Order and duplicates are preserved.
    /// </summary>
    public IReadOnlyList<string>? InjectJs { get; init; }

    /// <summary>Gets or initializes whether to disable the page's Content Security Policy.</summary>
    public bool? BypassCsp { get; init; }

    /// <summary>Gets or initializes the CSS selector for an element-only capture.</summary>
    public string? Selector { get; init; }

    /// <summary>Gets or initializes the clip rectangle's left coordinate, in pixels.</summary>
    public int? ClipX { get; init; }

    /// <summary>Gets or initializes the clip rectangle's top coordinate, in pixels.</summary>
    public int? ClipY { get; init; }

    /// <summary>Gets or initializes the clip rectangle's width, in pixels.</summary>
    public int? ClipWidth { get; init; }

    /// <summary>Gets or initializes the clip rectangle's height, in pixels.</summary>
    public int? ClipHeight { get; init; }

    /// <summary>Gets or initializes the final image width, in pixels.</summary>
    public int? ImageWidth { get; init; }

    /// <summary>Gets or initializes the final image height, in pixels.</summary>
    public int? ImageHeight { get; init; }

    /// <summary>Gets or initializes how the captured image is fitted into the requested dimensions.</summary>
    public CaptureImageMode? ImageMode { get; init; }

    /// <summary>Gets or initializes the anchor used when fitting or cropping the captured image.</summary>
    public CaptureImageAnchor? ImageAnchor { get; init; }

    /// <summary>Gets or initializes whether the captured image may be enlarged to the requested dimensions.</summary>
    public bool? ImageAllowUpscale { get; init; }

    /// <summary>Gets or initializes the canvas background as a <c>#RRGGBB</c> color.</summary>
    public string? ImageBackground { get; init; }

    /// <summary>Gets or initializes the compression quality for JPEG and WebP output.</summary>
    public int? ImageQuality { get; init; }

    /// <summary>Gets or initializes the paper size used for PDF output.</summary>
    public CapturePdfPaperFormat? PdfPaperFormat { get; init; }

    /// <summary>Gets or initializes whether PDF pages use landscape orientation.</summary>
    public bool? PdfLandscape { get; init; }

    /// <summary>Gets or initializes whether CSS backgrounds are printed in PDF output.</summary>
    public bool? PdfPrintBackground { get; init; }

    /// <summary>Gets or initializes the base PDF margin as a non-negative CSS length.</summary>
    public string? PdfMargin { get; init; }

    /// <summary>Gets or initializes the top PDF margin as a non-negative CSS length.</summary>
    public string? PdfMarginTop { get; init; }

    /// <summary>Gets or initializes the right PDF margin as a non-negative CSS length.</summary>
    public string? PdfMarginRight { get; init; }

    /// <summary>Gets or initializes the bottom PDF margin as a non-negative CSS length.</summary>
    public string? PdfMarginBottom { get; init; }

    /// <summary>Gets or initializes the left PDF margin as a non-negative CSS length.</summary>
    public string? PdfMarginLeft { get; init; }

    /// <summary>Gets or initializes the content scale factor used inside PDF pages.</summary>
    public double? PdfScale { get; init; }

    /// <summary>Gets or initializes whether identical requests may use Screenshot Scout storage caching.</summary>
    public bool? Cache { get; init; }

    /// <summary>Gets or initializes how long cached objects are retained, in seconds.</summary>
    public int? CacheTtl { get; init; }

    /// <summary>Gets or initializes a key that distinguishes cached variants with otherwise identical options.</summary>
    public string? CacheKey { get; init; }

    /// <summary>Gets or initializes whether screenshots use managed or external S3-compatible storage.</summary>
    public CaptureStorageMode? StorageMode { get; init; }

    /// <summary>Gets or initializes the HTTPS endpoint for external S3-compatible storage.</summary>
    public string? StorageEndpoint { get; init; }

    /// <summary>Gets or initializes the external storage bucket name.</summary>
    public string? StorageBucket { get; init; }

    /// <summary>Gets or initializes the external storage region.</summary>
    public string? StorageRegion { get; init; }

    /// <summary>Gets or initializes the external-storage object key without the output-format extension.</summary>
    public string? StorageObjectKey { get; init; }
}
