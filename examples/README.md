# Examples

Each directory contains a standalone console project that demonstrates one SDK operation. The binary and JSON capture examples perform asynchronous I/O and complete when Screenshot Scout returns the final response.

Set `SCREENSHOTSCOUT_ACCESS_KEY`.

## Binary capture

Captures a full-page screenshot and writes `screenshot.png` in the current directory:

```shell
dotnet run --project examples/BinaryCapture
```

## JSON capture

Requests a JSON response and prints the screenshot URL:

```shell
dotnet run --project examples/JsonCapture
```

## Build a capture URL

Builds and prints a capture URL without making an HTTP request. Set `SCREENSHOTSCOUT_SECRET_KEY` as well to sign it:

```shell
dotnet run --project examples/BuildCaptureUrl
```
