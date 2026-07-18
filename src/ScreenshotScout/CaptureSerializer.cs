using System.Globalization;
using System.Text;
using System.Text.Json;

namespace ScreenshotScout;

internal readonly record struct WirePair(string Name, string Value);

internal sealed class SerializedCaptureOptions
{
    internal SerializedCaptureOptions(List<WirePair> pairs, Dictionary<string, object> body)
    {
        Pairs = pairs;
        Body = body;
    }

    internal List<WirePair> Pairs { get; }
    internal Dictionary<string, object> Body { get; }
}

internal static class CaptureSerializer
{
    internal static SerializedCaptureOptions Serialize(string targetUrl, CaptureOptions? options)
    {
        try
        {
            return SerializeUnsafe(targetUrl, options);
        }
        catch (ScreenshotScoutException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new ScreenshotScoutSerializationException(
                "Capture options could not be serialized.",
                innerException: exception);
        }
    }

    internal static byte[] SerializeJsonBody(Dictionary<string, object> body)
    {
        try
        {
            return JsonSerializer.SerializeToUtf8Bytes(body);
        }
        catch (Exception exception)
        {
            throw new ScreenshotScoutSerializationException(
                "The POST capture body could not be serialized as JSON.",
                innerException: exception);
        }
    }

    internal static string BuildCanonicalQuery(IReadOnlyList<WirePair> pairs, string accessKey)
    {
        var entries = new List<(WirePair Pair, int Index)>(pairs.Count + 1);
        for (var index = 0; index < pairs.Count; index++)
        {
            entries.Add((pairs[index], index));
        }

        entries.Add((new WirePair("access_key", accessKey), pairs.Count));
        entries.Sort(static (left, right) =>
        {
            var nameOrder = StringComparer.Ordinal.Compare(left.Pair.Name, right.Pair.Name);
            return nameOrder != 0 ? nameOrder : left.Index.CompareTo(right.Index);
        });

        return EncodeQuery(entries.Select(static entry => entry.Pair));
    }

    internal static string EncodeQuery(IEnumerable<WirePair> pairs)
    {
        var builder = new StringBuilder();
        var first = true;
        foreach (var pair in pairs)
        {
            if (!first)
            {
                builder.Append('&');
            }

            first = false;
            builder.Append(WireFormatting.FormEncode(pair.Name));
            builder.Append('=');
            builder.Append(WireFormatting.FormEncode(pair.Value));
        }

        return builder.ToString();
    }

    private static SerializedCaptureOptions SerializeUnsafe(string targetUrl, CaptureOptions? options)
    {
        if (targetUrl is null)
        {
            throw new ScreenshotScoutSerializationException(
                "The capture target URL must be a string.",
                "url");
        }

        EnsureText(targetUrl, "url", "The capture target URL must contain valid Unicode text.");

        var pairs = new List<WirePair> { new("url", targetUrl) };
        var body = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            ["url"] = targetUrl,
        };
        if (options is null)
        {
            return new SerializedCaptureOptions(pairs, body);
        }

        var serializer = new OptionSerializer(pairs, body);

        if (options.Format is { } format)
        {
            serializer.AppendRequiredString("Format", "format", format.Value);
        }

        if (options.ResponseType is { } responseType)
        {
            serializer.AppendRequiredString("ResponseType", "response_type", responseType.Value);
        }

        serializer.AppendString("Country", "country", options.Country);
        serializer.AppendString("Proxy", "proxy", options.Proxy);
        serializer.AppendDouble("GeolocationLatitude", "geolocation_latitude", options.GeolocationLatitude);
        serializer.AppendDouble("GeolocationLongitude", "geolocation_longitude", options.GeolocationLongitude);
        serializer.AppendDouble("GeolocationAccuracy", "geolocation_accuracy", options.GeolocationAccuracy);
        serializer.AppendRepeated("Cookies", "cookies", options.Cookies);
        serializer.AppendRepeated("Headers", "headers", options.Headers);
        serializer.AppendInt("Timeout", "timeout", options.Timeout);
        if (options.WaitUntil is { } waitUntil)
        {
            serializer.AppendRequiredString("WaitUntil", "wait_until", waitUntil.Value);
        }

        serializer.AppendInt("NavigationTimeout", "navigation_timeout", options.NavigationTimeout);
        serializer.AppendInt("Delay", "delay", options.Delay);
        serializer.AppendString("Device", "device", options.Device);
        serializer.AppendInt("DeviceViewportWidth", "device_viewport_width", options.DeviceViewportWidth);
        serializer.AppendInt("DeviceViewportHeight", "device_viewport_height", options.DeviceViewportHeight);
        serializer.AppendDouble("DeviceScaleFactor", "device_scale_factor", options.DeviceScaleFactor);
        serializer.AppendBool("device_is_mobile", options.DeviceIsMobile);
        serializer.AppendBool("device_has_touch", options.DeviceHasTouch);
        serializer.AppendString("DeviceUserAgent", "device_user_agent", options.DeviceUserAgent);
        serializer.AppendString("Timezone", "timezone", options.Timezone);
        if (options.MediaType is { } mediaType)
        {
            serializer.AppendRequiredString("MediaType", "media_type", mediaType.Value);
        }

        if (options.ColorScheme is { } colorScheme)
        {
            serializer.AppendRequiredString("ColorScheme", "color_scheme", colorScheme.Value);
        }

        serializer.AppendBool("reduced_motion", options.ReducedMotion);
        serializer.AppendBool("full_page", options.FullPage);
        serializer.AppendBool("full_page_pre_scroll", options.FullPagePreScroll);
        serializer.AppendInt("FullPagePreScrollStep", "full_page_pre_scroll_step", options.FullPagePreScrollStep);
        serializer.AppendInt(
            "FullPagePreScrollStepDelay",
            "full_page_pre_scroll_step_delay",
            options.FullPagePreScrollStepDelay);
        serializer.AppendInt("FullPageMaxHeight", "full_page_max_height", options.FullPageMaxHeight);
        serializer.AppendBool("block_cookie_banners", options.BlockCookieBanners);
        serializer.AppendBool("block_ads", options.BlockAds);
        serializer.AppendBool("block_chat_widgets", options.BlockChatWidgets);
        serializer.AppendRepeated("HideSelectors", "hide_selectors", options.HideSelectors);
        serializer.AppendRepeated("ClickSelectors", "click_selectors", options.ClickSelectors);
        serializer.AppendRepeated("ClickAllSelectors", "click_all_selectors", options.ClickAllSelectors);
        serializer.AppendRepeated("InjectCss", "inject_css", options.InjectCss);
        serializer.AppendRepeated("InjectJs", "inject_js", options.InjectJs);
        serializer.AppendBool("bypass_csp", options.BypassCsp);
        serializer.AppendString("Selector", "selector", options.Selector);
        serializer.AppendInt("ClipX", "clip_x", options.ClipX);
        serializer.AppendInt("ClipY", "clip_y", options.ClipY);
        serializer.AppendInt("ClipWidth", "clip_width", options.ClipWidth);
        serializer.AppendInt("ClipHeight", "clip_height", options.ClipHeight);
        serializer.AppendInt("ImageWidth", "image_width", options.ImageWidth);
        serializer.AppendInt("ImageHeight", "image_height", options.ImageHeight);
        if (options.ImageMode is { } imageMode)
        {
            serializer.AppendRequiredString("ImageMode", "image_mode", imageMode.Value);
        }

        if (options.ImageAnchor is { } imageAnchor)
        {
            serializer.AppendRequiredString("ImageAnchor", "image_anchor", imageAnchor.Value);
        }

        serializer.AppendBool("image_allow_upscale", options.ImageAllowUpscale);
        serializer.AppendString("ImageBackground", "image_background", options.ImageBackground);
        serializer.AppendInt("ImageQuality", "image_quality", options.ImageQuality);
        if (options.PdfPaperFormat is { } pdfPaperFormat)
        {
            serializer.AppendRequiredString("PdfPaperFormat", "pdf_paper_format", pdfPaperFormat.Value);
        }

        serializer.AppendBool("pdf_landscape", options.PdfLandscape);
        serializer.AppendBool("pdf_print_background", options.PdfPrintBackground);
        serializer.AppendString("PdfMargin", "pdf_margin", options.PdfMargin);
        serializer.AppendString("PdfMarginTop", "pdf_margin_top", options.PdfMarginTop);
        serializer.AppendString("PdfMarginRight", "pdf_margin_right", options.PdfMarginRight);
        serializer.AppendString("PdfMarginBottom", "pdf_margin_bottom", options.PdfMarginBottom);
        serializer.AppendString("PdfMarginLeft", "pdf_margin_left", options.PdfMarginLeft);
        serializer.AppendDouble("PdfScale", "pdf_scale", options.PdfScale);
        serializer.AppendBool("cache", options.Cache);
        serializer.AppendInt("CacheTtl", "cache_ttl", options.CacheTtl);
        serializer.AppendString("CacheKey", "cache_key", options.CacheKey);
        if (options.StorageMode is { } storageMode)
        {
            serializer.AppendRequiredString("StorageMode", "storage_mode", storageMode.Value);
        }

        serializer.AppendString("StorageEndpoint", "storage_endpoint", options.StorageEndpoint);
        serializer.AppendString("StorageBucket", "storage_bucket", options.StorageBucket);
        serializer.AppendString("StorageRegion", "storage_region", options.StorageRegion);
        serializer.AppendString("StorageObjectKey", "storage_object_key", options.StorageObjectKey);

        return new SerializedCaptureOptions(pairs, body);
    }

    private static void EnsureText(string value, string option, string message)
    {
        if (!WireFormatting.IsValidUnicode(value))
        {
            throw new ScreenshotScoutSerializationException(message, option);
        }
    }

    private sealed class OptionSerializer
    {
        private readonly List<WirePair> _pairs;
        private readonly Dictionary<string, object> _body;

        internal OptionSerializer(List<WirePair> pairs, Dictionary<string, object> body)
        {
            _pairs = pairs;
            _body = body;
        }

        internal void AppendString(string property, string wireName, string? value)
        {
            if (value is null)
            {
                return;
            }

            AppendRequiredString(property, wireName, value);
        }

        internal void AppendRequiredString(string property, string wireName, string? value)
        {
            if (value is null || !WireFormatting.IsValidUnicode(value))
            {
                throw new ScreenshotScoutSerializationException(
                    $"The capture option \"{property}\" must contain valid Unicode text.",
                    property);
            }

            _pairs.Add(new WirePair(wireName, value));
            _body[wireName] = value;
        }

        internal void AppendRepeated(string property, string wireName, IReadOnlyList<string>? values)
        {
            if (values is null || values.Count == 0)
            {
                return;
            }

            var copy = new string[values.Count];
            for (var index = 0; index < values.Count; index++)
            {
                var value = values[index];
                if (value is null || !WireFormatting.IsValidUnicode(value))
                {
                    throw new ScreenshotScoutSerializationException(
                        $"The capture option \"{property}\" must contain only valid Unicode strings.",
                        property);
                }

                copy[index] = value;
                _pairs.Add(new WirePair(wireName, value));
            }

            _body[wireName] = copy;
        }

        internal void AppendInt(string property, string wireName, int? value)
        {
            if (value is null)
            {
                return;
            }

            _pairs.Add(new WirePair(wireName, value.Value.ToString(CultureInfo.InvariantCulture)));
            _body[wireName] = value.Value;
        }

        internal void AppendDouble(string property, string wireName, double? value)
        {
            if (value is null)
            {
                return;
            }

            if (!double.IsFinite(value.Value))
            {
                throw new ScreenshotScoutSerializationException(
                    $"The capture option \"{property}\" must be a finite number.",
                    property);
            }

            _pairs.Add(new WirePair(wireName, WireFormatting.FormatEcmaScriptNumber(value.Value)));
            _body[wireName] = value.Value == 0 ? 0d : value.Value;
        }

        internal void AppendBool(string wireName, bool? value)
        {
            if (value is null)
            {
                return;
            }

            _pairs.Add(new WirePair(wireName, value.Value ? "true" : "false"));
            _body[wireName] = value.Value;
        }
    }
}

internal static class WireFormatting
{
    private static readonly UTF8Encoding StrictUtf8 = new(
        encoderShouldEmitUTF8Identifier: false,
        throwOnInvalidBytes: true);

    internal static bool IsValidUnicode(string value)
    {
        try
        {
            _ = StrictUtf8.GetByteCount(value);
            return true;
        }
        catch (EncoderFallbackException)
        {
            return false;
        }
    }

    internal static string FormEncode(string value)
    {
        var bytes = StrictUtf8.GetBytes(value);
        var builder = new StringBuilder(bytes.Length);
        const string hex = "0123456789ABCDEF";
        foreach (var octet in bytes)
        {
            if ((octet >= (byte)'A' && octet <= (byte)'Z')
                || (octet >= (byte)'a' && octet <= (byte)'z')
                || (octet >= (byte)'0' && octet <= (byte)'9')
                || octet is (byte)'*' or (byte)'-' or (byte)'.' or (byte)'_')
            {
                builder.Append((char)octet);
            }
            else if (octet == (byte)' ')
            {
                builder.Append('+');
            }
            else
            {
                builder.Append('%');
                builder.Append(hex[octet >> 4]);
                builder.Append(hex[octet & 0x0F]);
            }
        }

        return builder.ToString();
    }

    internal static string FormatEcmaScriptNumber(double value)
    {
        if (value == 0)
        {
            return "0";
        }

        var negative = value < 0;
        var absolute = Math.Abs(value);
        var representation = absolute.ToString("R", CultureInfo.InvariantCulture);
        var exponentIndex = representation.IndexOfAny(['E', 'e']);
        var mantissa = exponentIndex < 0 ? representation : representation[..exponentIndex];
        var explicitExponent = exponentIndex < 0
            ? 0
            : int.Parse(representation[(exponentIndex + 1)..], CultureInfo.InvariantCulture);

        var decimalIndex = mantissa.IndexOf('.');
        var digitsBeforeDecimal = decimalIndex < 0 ? mantissa.Length : decimalIndex;
        var allDigits = decimalIndex < 0 ? mantissa : mantissa.Remove(decimalIndex, 1);
        var firstNonZero = 0;
        while (firstNonZero < allDigits.Length && allDigits[firstNonZero] == '0')
        {
            firstNonZero++;
        }

        var digits = allDigits[firstNonZero..].TrimEnd('0');
        if (digits.Length == 0)
        {
            return "0";
        }

        var decimalPosition = digitsBeforeDecimal + explicitExponent - firstNonZero;
        string formatted;
        if (absolute >= 1e-6 && absolute < 1e21)
        {
            if (decimalPosition <= 0)
            {
                formatted = "0." + new string('0', -decimalPosition) + digits;
            }
            else if (decimalPosition >= digits.Length)
            {
                formatted = digits + new string('0', decimalPosition - digits.Length);
            }
            else
            {
                formatted = digits.Insert(decimalPosition, ".");
            }
        }
        else
        {
            var exponent = decimalPosition - 1;
            var scientificMantissa = digits.Length == 1
                ? digits
                : digits[0] + "." + digits[1..];
            formatted = scientificMantissa
                + "e"
                + (exponent >= 0 ? "+" : string.Empty)
                + exponent.ToString(CultureInfo.InvariantCulture);
        }

        return negative ? "-" + formatted : formatted;
    }
}
