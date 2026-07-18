using ScreenshotScout;

var accessKey = Environment.GetEnvironmentVariable("SCREENSHOTSCOUT_ACCESS_KEY");
if (string.IsNullOrEmpty(accessKey))
{
    throw new InvalidOperationException("Set SCREENSHOTSCOUT_ACCESS_KEY first.");
}

var secretKey = Environment.GetEnvironmentVariable("SCREENSHOTSCOUT_SECRET_KEY");
using var client = new ScreenshotScoutClient(
    accessKey,
    string.IsNullOrEmpty(secretKey) ? null : secretKey);

var response = await client.CaptureAsync(
    "https://example.com",
    new CaptureOptions { FullPage = true });

if (response is not BinaryCaptureResponse binary)
{
    throw new InvalidOperationException("Expected a binary response.");
}

await File.WriteAllBytesAsync("screenshot.png", binary.Bytes);
Console.WriteLine(binary.ScreenshotUrl);
