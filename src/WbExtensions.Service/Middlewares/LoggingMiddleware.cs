using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WbExtensions.Service.Middlewares;

public sealed class LoggingMiddleware : IMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    private readonly string[] _excludedPaths = new[]
    {
        "swagger",
        "metrics"
    };

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Request.Headers.TryGetValue("X-Request-Id", out var requestId);
        using var scope = _logger.BeginScope("Request id = '{RequestId}'", requestId.ToString());

        var shouldLog = !_excludedPaths.Any(_ => context.Request.Path.ToString().Contains(_));

        var body = shouldLog
            ? await ReadBodyFromRequest(context.Request, context.RequestAborted)
            : string.Empty;

        await next(context);

        if (!shouldLog)
        {
            return;
        }

        context.Request.Headers.TryGetValue("Authorization", out var authHeader);
        
        _logger.LogInformation("{Method} {Path}{Query} - {StatusCode} - Body: {Body}.",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            context.Response.StatusCode,
            body);
    }

    private async Task<string> ReadBodyFromRequest(HttpRequest request, CancellationToken cancellationToken)
    {
        request.EnableBuffering();
        using var streamReader = new StreamReader(request.Body, leaveOpen: true);
        var requestBody = await streamReader.ReadToEndAsync(cancellationToken);
        request.Body.Position = 0;
        return requestBody;
    }
}