using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
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
        Prometheus.Metrics.DefaultRegistry.SetStaticLabels(
            new Dictionary<string, string>
            {
                ["service"] = "wbextensions"
            });

        services.AddSingleton<IMetricsService, MetricsService>();

        return services;
    }
}