namespace ScreenshotScout;

/// <summary>An open string-backed Screenshot Scout output format.</summary>
public readonly record struct CaptureFormat(string Value)
{
    public static CaptureFormat Gif { get; } = new("gif");
    public static CaptureFormat Jpeg { get; } = new("jpeg");
    public static CaptureFormat Jpg { get; } = new("jpg");
    public static CaptureFormat Pdf { get; } = new("pdf");
    public static CaptureFormat Png { get; } = new("png");
    public static CaptureFormat Tiff { get; } = new("tiff");
    public static CaptureFormat Webp { get; } = new("webp");

    public static implicit operator CaptureFormat(string value) => new(value);
    public static explicit operator string(CaptureFormat value) => value.Value;
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>An open string-backed successful-response representation.</summary>
public readonly record struct CaptureResponseType(string Value)
{
    public static CaptureResponseType Binary { get; } = new("binary");
    public static CaptureResponseType Json { get; } = new("json");

    public static implicit operator CaptureResponseType(string value) => new(value);
    public static explicit operator string(CaptureResponseType value) => value.Value;
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>An open string-backed navigation completion condition.</summary>
public readonly record struct CaptureWaitUntil(string Value)
{
    public static CaptureWaitUntil DomContentLoaded { get; } = new("domcontentloaded");
    public static CaptureWaitUntil Load { get; } = new("load");
    public static CaptureWaitUntil NetworkIdle0 { get; } = new("networkidle0");
    public static CaptureWaitUntil NetworkIdle2 { get; } = new("networkidle2");

    public static implicit operator CaptureWaitUntil(string value) => new(value);
    public static explicit operator string(CaptureWaitUntil value) => value.Value;
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>An open string-backed CSS media type.</summary>
public readonly record struct CaptureMediaType(string Value)
{
    public static CaptureMediaType Print { get; } = new("print");
    public static CaptureMediaType Screen { get; } = new("screen");

    public static implicit operator CaptureMediaType(string value) => new(value);
    public static explicit operator string(CaptureMediaType value) => value.Value;
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>An open string-backed emulated color scheme.</summary>
public readonly record struct CaptureColorScheme(string Value)
{
    public static CaptureColorScheme Auto { get; } = new("auto");
    public static CaptureColorScheme Dark { get; } = new("dark");
    public static CaptureColorScheme Light { get; } = new("light");

    public static implicit operator CaptureColorScheme(string value) => new(value);
    public static explicit operator string(CaptureColorScheme value) => value.Value;
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>An open string-backed image resize mode.</summary>
public readonly record struct CaptureImageMode(string Value)
{
    public static CaptureImageMode Fill { get; } = new("fill");
    public static CaptureImageMode Fit { get; } = new("fit");
    public static CaptureImageMode Stretch { get; } = new("stretch");

    public static implicit operator CaptureImageMode(string value) => new(value);
    public static explicit operator string(CaptureImageMode value) => value.Value;
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>An open string-backed image resize anchor.</summary>
public readonly record struct CaptureImageAnchor(string Value)
{
    public static CaptureImageAnchor Bottom { get; } = new("bottom");
    public static CaptureImageAnchor BottomLeft { get; } = new("bottom_left");
    public static CaptureImageAnchor BottomRight { get; } = new("bottom_right");
    public static CaptureImageAnchor Center { get; } = new("center");
    public static CaptureImageAnchor Left { get; } = new("left");
    public static CaptureImageAnchor Right { get; } = new("right");
    public static CaptureImageAnchor Top { get; } = new("top");
    public static CaptureImageAnchor TopLeft { get; } = new("top_left");
    public static CaptureImageAnchor TopRight { get; } = new("top_right");

    public static implicit operator CaptureImageAnchor(string value) => new(value);
    public static explicit operator string(CaptureImageAnchor value) => value.Value;
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>An open string-backed PDF paper format.</summary>
public readonly record struct CapturePdfPaperFormat(string Value)
{
    public static CapturePdfPaperFormat A3 { get; } = new("a3");
    public static CapturePdfPaperFormat A4 { get; } = new("a4");
    public static CapturePdfPaperFormat Content { get; } = new("content");
    public static CapturePdfPaperFormat Legal { get; } = new("legal");
    public static CapturePdfPaperFormat Letter { get; } = new("letter");
    public static CapturePdfPaperFormat Tabloid { get; } = new("tabloid");

    public static implicit operator CapturePdfPaperFormat(string value) => new(value);
    public static explicit operator string(CapturePdfPaperFormat value) => value.Value;
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>An open string-backed screenshot storage mode.</summary>
public readonly record struct CaptureStorageMode(string Value)
{
    public static CaptureStorageMode External { get; } = new("external");
    public static CaptureStorageMode Managed { get; } = new("managed");

    public static implicit operator CaptureStorageMode(string value) => new(value);
    public static explicit operator string(CaptureStorageMode value) => value.Value;
    public override string ToString() => Value ?? string.Empty;
}
