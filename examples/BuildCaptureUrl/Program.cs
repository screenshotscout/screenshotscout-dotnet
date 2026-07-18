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

var captureUrl = client.BuildCaptureUrl(
    "https://example.com",
    new CaptureOptions
    {
        FullPage = true,
        BlockAds = true,
    });

Console.WriteLine(captureUrl);
