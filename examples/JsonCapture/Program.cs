using ScreenshotScout;

using var client = new ScreenshotScoutClient(); // Reads SCREENSHOTSCOUT_ACCESS_KEY

var response = await client.CaptureAsync(
    "https://example.com",
    new CaptureOptions
    {
        ResponseType = CaptureResponseType.Json,
    });

if (response is not JsonCaptureResponse json)
{
    throw new InvalidOperationException("Expected a JSON response.");
}

Console.WriteLine(json.Result.ScreenshotUrl);
