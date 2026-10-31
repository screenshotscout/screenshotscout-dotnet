using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

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
    public void QueryEncodingFollowsRfc3986()
    {
        using var client = new ScreenshotScoutClient("key");

        var url = client.BuildCaptureUrl("https://example.com/a path?x=-._~*!'()+&emoji=😀");

        Assert.Equal(
            "https://api.screenshotscout.com/v1/capture?access_key=key&url=https%3A%2F%2Fexample.com%2Fa%20path%3Fx%3D-._~%2A%21%27%28%29%2B%26emoji%3D%F0%9F%98%80",
            url);
    }

    public static TheoryData<string> SigningVectorNames()
    {
        var names = new TheoryData<string>();
        foreach (var vector in ReadSigningVectors())
        {
            names.Add(vector.GetProperty("name").GetString()!);
        }

        return names;
    }

    [Theory]
    [MemberData(nameof(SigningVectorNames))]
    public void SignedTextMatchesTheSharedSigningVector(string name)
    {
        var vector = ReadSigningVectors().Single(
            candidate => candidate.GetProperty("name").GetString() == name);
        var pairs = vector.GetProperty("params").EnumerateArray()
            .Select(static pair => new WirePair(pair[0].GetString()!, pair[1].GetString()!))
            .ToList();

        var signedText = CaptureSerializer.BuildCanonicalQuery(
            pairs,
            vector.GetProperty("access_key").GetString()!);
        var signature = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(vector.GetProperty("secret_key").GetString()!),
            Encoding.UTF8.GetBytes(signedText));

        Assert.Equal(vector.GetProperty("signed_text").GetString(), signedText);
        Assert.Equal(
            vector.GetProperty("signature").GetString(),
            Convert.ToHexString(signature).ToLowerInvariant());
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

    private static List<JsonElement> ReadSigningVectors()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "signing-vectors.json");
        using var document = JsonDocument.Parse(File.ReadAllText(path, Encoding.UTF8));
        return document.RootElement.GetProperty("vectors").EnumerateArray()
            .Select(static vector => vector.Clone())
            .ToList();
    }
}
