namespace ScreenshotScout;

/// <summary>Represents an open string-backed Screenshot Scout output format.</summary>
/// <param name="Value">The raw service value.</param>
public readonly record struct CaptureFormat(string Value)
{
    /// <summary>Gets the raw service value.</summary>
    public string Value { get; init; } = Value;

    /// <summary>Gets the <c>gif</c> output format.</summary>
    public static CaptureFormat Gif { get; } = new("gif");

    /// <summary>Gets the <c>jpeg</c> alias for the JPEG output format.</summary>
    public static CaptureFormat Jpeg { get; } = new("jpeg");

    /// <summary>Gets the <c>jpg</c> JPEG output format.</summary>
    public static CaptureFormat Jpg { get; } = new("jpg");

    /// <summary>Gets the <c>pdf</c> output format.</summary>
    public static CaptureFormat Pdf { get; } = new("pdf");

    /// <summary>Gets the <c>png</c> output format.</summary>
    public static CaptureFormat Png { get; } = new("png");

    /// <summary>Gets the <c>tiff</c> output format.</summary>
    public static CaptureFormat Tiff { get; } = new("tiff");

    /// <summary>Gets the <c>webp</c> output format.</summary>
    public static CaptureFormat Webp { get; } = new("webp");

    /// <summary>Creates a format from a raw service value.</summary>
    /// <param name="value">The raw service value.</param>
    public static explicit operator CaptureFormat(string value) => new(value);

    /// <summary>Returns the raw service value.</summary>
    /// <param name="value">The format to convert.</param>
    public static explicit operator string(CaptureFormat value) => value.Value;

    /// <summary>Returns the raw service value.</summary>
    /// <returns>The raw service value, or an empty string for the default value.</returns>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>Represents an open string-backed successful-response representation.</summary>
/// <param name="Value">The raw service value.</param>
public readonly record struct CaptureResponseType(string Value)
{
    /// <summary>Gets the raw service value.</summary>
    public string Value { get; init; } = Value;

    /// <summary>Gets the response type that returns the screenshot or PDF body directly.</summary>
    public static CaptureResponseType Binary { get; } = new("binary");

    /// <summary>Gets the response type that returns a JSON result.</summary>
    public static CaptureResponseType Json { get; } = new("json");

    /// <summary>Creates a response type from a raw service value.</summary>
    /// <param name="value">The raw service value.</param>
    public static explicit operator CaptureResponseType(string value) => new(value);

    /// <summary>Returns the raw service value.</summary>
    /// <param name="value">The response type to convert.</param>
    public static explicit operator string(CaptureResponseType value) => value.Value;

    /// <summary>Returns the raw service value.</summary>
    /// <returns>The raw service value, or an empty string for the default value.</returns>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>Represents an open string-backed navigation completion condition.</summary>
/// <param name="Value">The raw service value.</param>
public readonly record struct CaptureWaitUntil(string Value)
{
    /// <summary>Gets the raw service value.</summary>
    public string Value { get; init; } = Value;

    /// <summary>Gets the condition that waits for the <c>DOMContentLoaded</c> event.</summary>
    public static CaptureWaitUntil DomContentLoaded { get; } = new("domcontentloaded");

    /// <summary>Gets the condition that waits for the page <c>load</c> event.</summary>
    public static CaptureWaitUntil Load { get; } = new("load");

    /// <summary>Gets the condition that waits until no network connections remain.</summary>
    public static CaptureWaitUntil NetworkIdle0 { get; } = new("networkidle0");

    /// <summary>Gets the condition that waits until no more than two network connections remain.</summary>
    public static CaptureWaitUntil NetworkIdle2 { get; } = new("networkidle2");

    /// <summary>Creates a navigation condition from a raw service value.</summary>
    /// <param name="value">The raw service value.</param>
    public static explicit operator CaptureWaitUntil(string value) => new(value);

    /// <summary>Returns the raw service value.</summary>
    /// <param name="value">The navigation condition to convert.</param>
    public static explicit operator string(CaptureWaitUntil value) => value.Value;

    /// <summary>Returns the raw service value.</summary>
    /// <returns>The raw service value, or an empty string for the default value.</returns>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>Represents an open string-backed CSS media type.</summary>
/// <param name="Value">The raw service value.</param>
public readonly record struct CaptureMediaType(string Value)
{
    /// <summary>Gets the raw service value.</summary>
    public string Value { get; init; } = Value;

    /// <summary>Gets the CSS print media type.</summary>
    public static CaptureMediaType Print { get; } = new("print");

    /// <summary>Gets the CSS screen media type.</summary>
    public static CaptureMediaType Screen { get; } = new("screen");

    /// <summary>Creates a media type from a raw service value.</summary>
    /// <param name="value">The raw service value.</param>
    public static explicit operator CaptureMediaType(string value) => new(value);

    /// <summary>Returns the raw service value.</summary>
    /// <param name="value">The media type to convert.</param>
    public static explicit operator string(CaptureMediaType value) => value.Value;

    /// <summary>Returns the raw service value.</summary>
    /// <returns>The raw service value, or an empty string for the default value.</returns>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>Represents an open string-backed emulated color scheme.</summary>
/// <param name="Value">The raw service value.</param>
public readonly record struct CaptureColorScheme(string Value)
{
    /// <summary>Gets the raw service value.</summary>
    public string Value { get; init; } = Value;

    /// <summary>Gets automatic color-scheme selection.</summary>
    public static CaptureColorScheme Auto { get; } = new("auto");

    /// <summary>Gets the dark color scheme.</summary>
    public static CaptureColorScheme Dark { get; } = new("dark");

    /// <summary>Gets the light color scheme.</summary>
    public static CaptureColorScheme Light { get; } = new("light");

    /// <summary>Creates a color scheme from a raw service value.</summary>
    /// <param name="value">The raw service value.</param>
    public static explicit operator CaptureColorScheme(string value) => new(value);

    /// <summary>Returns the raw service value.</summary>
    /// <param name="value">The color scheme to convert.</param>
    public static explicit operator string(CaptureColorScheme value) => value.Value;

    /// <summary>Returns the raw service value.</summary>
    /// <returns>The raw service value, or an empty string for the default value.</returns>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>Represents an open string-backed image resize mode.</summary>
/// <param name="Value">The raw service value.</param>
public readonly record struct CaptureImageMode(string Value)
{
    /// <summary>Gets the raw service value.</summary>
    public string Value { get; init; } = Value;

    /// <summary>Gets the mode that fills the target box while preserving aspect ratio and cropping as needed.</summary>
    public static CaptureImageMode Fill { get; } = new("fill");

    /// <summary>Gets the mode that fits the full image inside the target box while preserving aspect ratio.</summary>
    public static CaptureImageMode Fit { get; } = new("fit");

    /// <summary>Gets the mode that stretches the image to the target dimensions.</summary>
    public static CaptureImageMode Stretch { get; } = new("stretch");

    /// <summary>Creates an image resize mode from a raw service value.</summary>
    /// <param name="value">The raw service value.</param>
    public static explicit operator CaptureImageMode(string value) => new(value);

    /// <summary>Returns the raw service value.</summary>
    /// <param name="value">The image resize mode to convert.</param>
    public static explicit operator string(CaptureImageMode value) => value.Value;

    /// <summary>Returns the raw service value.</summary>
    /// <returns>The raw service value, or an empty string for the default value.</returns>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>Represents an open string-backed image resize anchor.</summary>
/// <param name="Value">The raw service value.</param>
public readonly record struct CaptureImageAnchor(string Value)
{
    /// <summary>Gets the raw service value.</summary>
    public string Value { get; init; } = Value;

    /// <summary>Gets the bottom anchor.</summary>
    public static CaptureImageAnchor Bottom { get; } = new("bottom");

    /// <summary>Gets the bottom-left anchor.</summary>
    public static CaptureImageAnchor BottomLeft { get; } = new("bottom_left");

    /// <summary>Gets the bottom-right anchor.</summary>
    public static CaptureImageAnchor BottomRight { get; } = new("bottom_right");

    /// <summary>Gets the center anchor.</summary>
    public static CaptureImageAnchor Center { get; } = new("center");

    /// <summary>Gets the left anchor.</summary>
    public static CaptureImageAnchor Left { get; } = new("left");

    /// <summary>Gets the right anchor.</summary>
    public static CaptureImageAnchor Right { get; } = new("right");

    /// <summary>Gets the top anchor.</summary>
    public static CaptureImageAnchor Top { get; } = new("top");

    /// <summary>Gets the top-left anchor.</summary>
    public static CaptureImageAnchor TopLeft { get; } = new("top_left");

    /// <summary>Gets the top-right anchor.</summary>
    public static CaptureImageAnchor TopRight { get; } = new("top_right");

    /// <summary>Creates an image anchor from a raw service value.</summary>
    /// <param name="value">The raw service value.</param>
    public static explicit operator CaptureImageAnchor(string value) => new(value);

    /// <summary>Returns the raw service value.</summary>
    /// <param name="value">The image anchor to convert.</param>
    public static explicit operator string(CaptureImageAnchor value) => value.Value;

    /// <summary>Returns the raw service value.</summary>
    /// <returns>The raw service value, or an empty string for the default value.</returns>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>Represents an open string-backed PDF paper format.</summary>
/// <param name="Value">The raw service value.</param>
public readonly record struct CapturePdfPaperFormat(string Value)
{
    /// <summary>Gets the A3 paper format.</summary>
    public static CapturePdfPaperFormat A3 { get; } = new("a3");

    /// <summary>Gets the A4 paper format.</summary>
    public static CapturePdfPaperFormat A4 { get; } = new("a4");

    /// <summary>Gets the content-sized PDF format.</summary>
    public static CapturePdfPaperFormat Content { get; } = new("content");

    /// <summary>Gets the legal paper format.</summary>
    public static CapturePdfPaperFormat Legal { get; } = new("legal");

    /// <summary>Gets the letter paper format.</summary>
    public static CapturePdfPaperFormat Letter { get; } = new("letter");

    /// <summary>Gets the tabloid paper format.</summary>
    public static CapturePdfPaperFormat Tabloid { get; } = new("tabloid");

    /// <summary>Gets the raw service value.</summary>
    public string Value { get; init; } = Value;

    /// <summary>Creates a PDF paper format from a raw service value.</summary>
    /// <param name="value">The raw service value.</param>
    public static explicit operator CapturePdfPaperFormat(string value) => new(value);

    /// <summary>Returns the raw service value.</summary>
    /// <param name="value">The PDF paper format to convert.</param>
    public static explicit operator string(CapturePdfPaperFormat value) => value.Value;

    /// <summary>Returns the raw service value.</summary>
    /// <returns>The raw service value, or an empty string for the default value.</returns>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>Represents an open string-backed screenshot storage mode.</summary>
/// <param name="Value">The raw service value.</param>
public readonly record struct CaptureStorageMode(string Value)
{
    /// <summary>Gets the raw service value.</summary>
    public string Value { get; init; } = Value;

    /// <summary>Gets external S3-compatible storage.</summary>
    public static CaptureStorageMode External { get; } = new("external");

    /// <summary>Gets Screenshot Scout managed storage.</summary>
    public static CaptureStorageMode Managed { get; } = new("managed");

    /// <summary>Creates a storage mode from a raw service value.</summary>
    /// <param name="value">The raw service value.</param>
    public static explicit operator CaptureStorageMode(string value) => new(value);

    /// <summary>Returns the raw service value.</summary>
    /// <param name="value">The storage mode to convert.</param>
    public static explicit operator string(CaptureStorageMode value) => value.Value;

    /// <summary>Returns the raw service value.</summary>
    /// <returns>The raw service value, or an empty string for the default value.</returns>
    public override string ToString() => Value ?? string.Empty;
}
