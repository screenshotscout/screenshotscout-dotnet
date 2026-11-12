using ScreenshotScout;

using var client = new ScreenshotScoutClient(); // Reads SCREENSHOTSCOUT_ACCESS_KEY

var response = await client.CaptureAsync(
    "https://example.com",
    new CaptureOptions { FullPage = true });

if (response is not BinaryCaptureResponse binary)
{
    throw new InvalidOperationException("Expected a binary response.");
}

await File.WriteAllBytesAsync("screenshot.png", binary.Bytes);
Console.WriteLine(binary.ScreenshotUrl);
