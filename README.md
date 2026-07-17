# Screenshot Scout .NET SDK

The official .NET SDK for [Screenshot Scout](https://screenshotscout.com).

> Status: initial local scaffold. The client is not implemented or published yet.

The package targets `net8.0`. Initial compatibility verification will cover .NET 8, 9, and 10.

## Repository layout

- `src/ScreenshotScout` contains the `ScreenshotScout` NuGet package project.
- `tests/ScreenshotScout.Tests` contains focused SDK tests.
- `examples` will contain runnable usage examples when the client is implemented.

## Development

```shell
dotnet restore ScreenshotScout.sln
dotnet build ScreenshotScout.sln --configuration Release --no-restore
dotnet test ScreenshotScout.sln --configuration Release --no-build
```

## License

Licensed under the [MIT License](LICENSE).
