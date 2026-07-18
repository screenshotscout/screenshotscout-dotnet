namespace ScreenshotScout;

/// <summary>
/// Contains Screenshot Scout service options. The target URL, credentials,
/// signature, request method, and cancellation token are deliberately separate.
/// </summary>
public sealed class CaptureOptions
{
    // Target and output.
    public CaptureFormat? Format { get; init; }
    public CaptureResponseType? ResponseType { get; init; }

    // Network and location.
    public string? Country { get; init; }
    public string? Proxy { get; init; }
    public double? GeolocationLatitude { get; init; }
    public double? GeolocationLongitude { get; init; }
    public double? GeolocationAccuracy { get; init; }

    // Cookies and webpage request headers. Order and duplicates are preserved.
    public IReadOnlyList<string>? Cookies { get; init; }
    public IReadOnlyList<string>? Headers { get; init; }

    // Navigation and service-side timing.
    public int? Timeout { get; init; }
    public CaptureWaitUntil? WaitUntil { get; init; }
    public int? NavigationTimeout { get; init; }
    public int? Delay { get; init; }

    // Viewport and device emulation.
    public string? Device { get; init; }
    public int? DeviceViewportWidth { get; init; }
    public int? DeviceViewportHeight { get; init; }
    public double? DeviceScaleFactor { get; init; }
    public bool? DeviceIsMobile { get; init; }
    public bool? DeviceHasTouch { get; init; }
    public string? DeviceUserAgent { get; init; }

    // Page behavior and preferences.
    public string? Timezone { get; init; }
    public CaptureMediaType? MediaType { get; init; }
    public CaptureColorScheme? ColorScheme { get; init; }
    public bool? ReducedMotion { get; init; }

    // Full-page capture and pre-scroll behavior.
    public bool? FullPage { get; init; }
    public bool? FullPagePreScroll { get; init; }
    public int? FullPagePreScrollStep { get; init; }
    public int? FullPagePreScrollStepDelay { get; init; }
    public int? FullPageMaxHeight { get; init; }

    // Blocking.
    public bool? BlockCookieBanners { get; init; }
    public bool? BlockAds { get; init; }
    public bool? BlockChatWidgets { get; init; }

    // DOM interactions and injections. Order and duplicates are preserved.
    public IReadOnlyList<string>? HideSelectors { get; init; }
    public IReadOnlyList<string>? ClickSelectors { get; init; }
    public IReadOnlyList<string>? ClickAllSelectors { get; init; }
    public IReadOnlyList<string>? InjectCss { get; init; }
    public IReadOnlyList<string>? InjectJs { get; init; }
    public bool? BypassCsp { get; init; }

    // Framing and selection.
    public string? Selector { get; init; }
    public int? ClipX { get; init; }
    public int? ClipY { get; init; }
    public int? ClipWidth { get; init; }
    public int? ClipHeight { get; init; }

    // Image resizing and quality.
    public int? ImageWidth { get; init; }
    public int? ImageHeight { get; init; }
    public CaptureImageMode? ImageMode { get; init; }
    public CaptureImageAnchor? ImageAnchor { get; init; }
    public bool? ImageAllowUpscale { get; init; }
    public string? ImageBackground { get; init; }
    public int? ImageQuality { get; init; }

    // PDF.
    public CapturePdfPaperFormat? PdfPaperFormat { get; init; }
    public bool? PdfLandscape { get; init; }
    public bool? PdfPrintBackground { get; init; }
    public string? PdfMargin { get; init; }
    public string? PdfMarginTop { get; init; }
    public string? PdfMarginRight { get; init; }
    public string? PdfMarginBottom { get; init; }
    public string? PdfMarginLeft { get; init; }
    public double? PdfScale { get; init; }

    // Caching.
    public bool? Cache { get; init; }
    public int? CacheTtl { get; init; }
    public string? CacheKey { get; init; }

    // Storage.
    public CaptureStorageMode? StorageMode { get; init; }
    public string? StorageEndpoint { get; init; }
    public string? StorageBucket { get; init; }
    public string? StorageRegion { get; init; }
    public string? StorageObjectKey { get; init; }
}
