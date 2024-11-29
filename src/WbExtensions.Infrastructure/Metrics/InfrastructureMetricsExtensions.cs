using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Prometheus;
using WbExtensions.Application.Interfaces.Metrics;

namespace WbExtensions.Infrastructure.Metrics;

public static class InfrastructureMetricsExtensions
{
    public static IEndpointRouteBuilder AddMetricsPullHost(
        this IEndpointRouteBuilder builder)
    {
        builder.MapMetrics();

        return builder;
    }

    internal static IServiceCollection SetupMetrics(
        this IServiceCollection services)
    {
        var metricsService = new MetricsService();

        Prometheus.Metrics.DefaultRegistry.SetStaticLabels(
            new Dictionary<string, string>
            {
                ["service"] = "wbextensions"
            });

        services
            .AddOpenTelemetry()
            .ConfigureResource(builder => builder
                .AddService("WbExtensions")
                .AddAttributes( new List<KeyValuePair<string, object>>
                {
                    new("service", "wbextensions")
                })
                .AddEnvironmentVariableDetector()
                .Build())
            .WithMetrics(builder => builder
                .AddMeter(metricsService.MeterName)
                .AddAspNetCoreInstrumentation()
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                .AddMeter("Microsoft.AspNetCore.Http.Connections")
                .AddMeter("Microsoft.AspNetCore.Routing")
                .AddMeter("Microsoft.AspNetCore.Diagnostics")
                .AddMeter("Microsoft.AspNetCore.RateLimiting")
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddPrometheusExporter());

        services.AddSingleton<IMetricsService>(metricsService);

        return services;
    }
}