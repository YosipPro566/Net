 using System.Diagnostics;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddOcelot()
    .AddEnvironmentVariables();

builder.Services.AddCors();
builder.Services.AddOcelot(builder.Configuration);

var allowedOrigin = builder.Configuration["Cors:AllowedOrigin"] ?? "http://localhost:5193";
var app = builder.Build();


app.Use(async (context, next) =>
{
    var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault();
    if (string.IsNullOrWhiteSpace(correlationId))
    {
        correlationId = Guid.NewGuid().ToString("D");
    }
    context.Request.Headers["X-Correlation-Id"] = correlationId;
    context.Response.Headers["X-Correlation-Id"] = correlationId;

    var started = Stopwatch.GetTimestamp();
    var method = context.Request.Method;
    var path = context.Request.Path + context.Request.QueryString;
    var isHealth = context.Request.Path.Equals("/health", StringComparison.OrdinalIgnoreCase);
    var targetHost = isHealth ? "gateway" : builder.Configuration["Monolith:Host"] ?? "localhost";
    var targetPort = isHealth ? "self" : builder.Configuration["Monolith:Port"] ?? "5193";

    try
    {
        if (isHealth)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { status = "Healthy", service = "ApiGateway" });
            return;
        }

        await next();
    }
    finally
    {
        var elapsedMs = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
        app.Logger.LogInformation(
            "Gateway request {Method} {Path} -> {Host}:{Port}; status={StatusCode}; durationMs={DurationMs}; correlationId={CorrelationId}",
            method, path, targetHost, targetPort, context.Response.StatusCode,
            Math.Round(elapsedMs, 2), correlationId);
    }
});

app.UseCors(policy => policy
    .WithOrigins(allowedOrigin)
    .AllowAnyHeader()
    .AllowAnyMethod());

await app.UseOcelot();
await app.RunAsync();
