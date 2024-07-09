using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;
using WbExtensions.Domain.Home;

namespace WbExtensions.Infrastructure.Home;

internal static class InfrastructureHomeExtensions
{
    public static IServiceCollection SetupHome(this IServiceCollection services, IConfiguration configuration)
    {
        var schema = configuration
                         .GetSection("Schema")
                         .Get<DevicesSchema>()
                     ?? throw new ArgumentNullException(nameof(DevicesSchema));
        services
            .AddSingleton(schema);

        return services;
    }
}