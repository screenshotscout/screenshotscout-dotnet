using System.Text;

using Xunit;

namespace ScreenshotScout.Tests;

public sealed class SerializationTests
{
    [Theory]
    [InlineData(0d, "0")]
    [InlineData(1.25d, "1.25")]
    [InlineData(0.000001d, "0.000001")]
    [InlineData(0.0000001d, "1e-7")]
    [InlineData(100000000000000000000d, "100000000000000000000")]
    [InlineData(1e21d, "1e%2B21")]
    [InlineData(-1.2e-7d, "-1.2e-7")]
    public void QueryNumbersMatchEcmaScriptFormatting(double value, string encodedValue)
    {
        using var client = new ScreenshotScoutClient("key");

        var url = client.BuildCaptureUrl(
            "https://example.com",
            new CaptureOptions { PdfScale = value });

        Assert.EndsWith("&pdf_scale=" + encodedValue, url, StringComparison.Ordinal);
    }

    [Fact]
    public void FormEncodingMatchesUrlSearchParamsRules()
    {
        using var client = new ScreenshotScoutClient("key");

        var url = client.BuildCaptureUrl("https://example.com/a path?x=~*&emoji=😀");

        Assert.Equal(
            "https://api.screenshotscout.com/v1/capture?access_key=key&url=https%3A%2F%2Fexample.com%2Fa+path%3Fx%3D%7E*%26emoji%3D%F0%9F%98%80",
            url);
    }

    [Fact]
    public async Task NegativeZeroUsesTheJavaScriptQueryAndJsonRepresentation()
    {
        using var transport = new TestTransport();
        using var client = new ScreenshotScoutClient(
            "key",
            options: new ScreenshotScoutClientOptions { HttpClient = transport.Client });
        var negativeZero = double.CopySign(0, -1);

        _ = await client.CaptureAsync(
            "https://example.com",
            new CaptureOptions { PdfScale = negativeZero },
            TestContext.Current.CancellationToken);
        var url = client.BuildCaptureUrl(
            "https://example.com",
            new CaptureOptions { PdfScale = negativeZero });

        Assert.Contains("\"pdf_scale\":0", Encoding.UTF8.GetString(transport.Requests[0].Body));
        Assert.EndsWith("&pdf_scale=0", url, StringComparison.Ordinal);
    }
}
