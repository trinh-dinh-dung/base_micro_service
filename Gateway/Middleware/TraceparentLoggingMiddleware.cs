namespace Gateway.Middleware;

public sealed class TraceparentLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TraceparentLoggingMiddleware> _logger;

    public TraceparentLoggingMiddleware(RequestDelegate next, ILogger<TraceparentLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var traceparent = context.Request.Headers["traceparent"].FirstOrDefault();
        if (!string.IsNullOrEmpty(traceparent))
        {
            _logger.LogInformation(
                "[Gateway] {Method} {Path} traceparent={Traceparent}",
                context.Request.Method,
                context.Request.Path,
                traceparent);
        }

        await _next(context);
    }
}
