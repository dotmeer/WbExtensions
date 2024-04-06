using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WbExtensions.Service.Middlewares;

public sealed class ExceptionsMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionsMiddleware> _logger;

    public ExceptionsMiddleware(ILogger<ExceptionsMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError(ex, "Unhandled exception");
        }
    }
}