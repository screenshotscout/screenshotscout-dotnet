using System.Net;

namespace ScreenshotScout.Tests;

internal sealed record RecordedRequest(
    string Method,
    string Url,
    string? Authorization,
    string? ContentType,
    byte[] Body,
    CancellationToken CancellationToken);

internal sealed class TestTransport : IDisposable
{
    private readonly RecordingHandler _handler;

    internal TestTransport(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>? responder = null)
    {
        _handler = new RecordingHandler(
            responder
            ?? ((_, _) => Task.FromResult(Response(
                200,
                "OK",
                "image/png",
                [1]))));
        Client = new HttpClient(_handler, disposeHandler: true);
    }

    internal HttpClient Client { get; }
    internal IReadOnlyList<RecordedRequest> Requests => _handler.Requests;
    internal bool HandlerDisposed => _handler.IsDisposed;

    public void Dispose()
    {
        Client.Dispose();
    }

    internal static HttpResponseMessage Response(
        int statusCode,
        string? reasonPhrase,
        string? contentType,
        byte[] body,
        IReadOnlyDictionary<string, IReadOnlyList<string>>? headers = null)
    {
        var response = new HttpResponseMessage((HttpStatusCode)statusCode)
        {
            ReasonPhrase = reasonPhrase,
            Content = new ByteArrayContent(body),
        };
        if (contentType is not null)
        {
            response.Content.Headers.TryAddWithoutValidation("Content-Type", contentType);
        }

        if (headers is not null)
        {
            foreach (var (name, values) in headers)
            {
                response.Headers.TryAddWithoutValidation(name, values);
            }
        }

        return response;
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _responder;

        internal RecordingHandler(
            Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder)
        {
            _responder = responder;
        }

        internal List<RecordedRequest> Requests { get; } = [];
        internal bool IsDisposed { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var body = request.Content is null
                ? []
                : await request.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            Requests.Add(new RecordedRequest(
                request.Method.Method,
                request.RequestUri?.OriginalString ?? string.Empty,
                request.Headers.Authorization?.ToString(),
                request.Content?.Headers.ContentType?.ToString(),
                body,
                cancellationToken));
            return await _responder(request, cancellationToken).ConfigureAwait(false);
        }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }
}
