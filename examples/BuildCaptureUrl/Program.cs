using ScreenshotScout;

// Reads SCREENSHOTSCOUT_ACCESS_KEY, and SCREENSHOTSCOUT_SECRET_KEY if it's set
using var client = new ScreenshotScoutClient();

var captureUrl = client.BuildCaptureUrl(
    "https://example.com",
    new CaptureOptions
    {
        FullPage = true,
        BlockAds = true,
    });

Console.WriteLine(captureUrl);
