using ScreenshotScout;

var accessKey = Environment.GetEnvironmentVariable("SCREENSHOTSCOUT_ACCESS_KEY");
if (string.IsNullOrEmpty(accessKey))
{
    throw new InvalidOperationException("Set SCREENSHOTSCOUT_ACCESS_KEY first.");
}

using var client = new ScreenshotScoutClient(accessKey);

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
