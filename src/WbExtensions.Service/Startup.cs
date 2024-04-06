using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using WbExtensions.Application;
using WbExtensions.Application.MqttHandlers;
using WbExtensions.Domain.Mqtt;
using WbExtensions.Infrastructure.Json;
using WbExtensions.Infrastructure.Metrics;
using WbExtensions.Infrastructure.Mqtt;
using WbExtensions.Infrastructure.Yandex;
using WbExtensions.Service.Authorization;
using WbExtensions.Service.Middlewares;

namespace WbExtensions.Service;

internal sealed class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddControllers()
            .AddControllersAsServices()
            .AddJsonOptions(_ => _.JsonSerializerOptions.Configure());

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Default", Version = "v1" });
            options.CustomSchemaIds(_ => _.FullName);
            options.UseAllOfToExtendReferenceSchemas();
            options.SupportNonNullableReferenceTypes();
        });

        services.AddRouting(options => { options.AppendTrailingSlash = true; });
        
        services
            .AddAuthentication()
            .AddScheme<TokenAuthenticationOptions, TokenAuthenticationHandler>(AuthConstants.SchemeName, _ => { });

        services
            .AddAuthorization(options =>
            {
                options.AddPolicy(AuthConstants.SchemeName, policy =>
                    policy.RequireClaim(AuthConstants.UserIdClaim));
            });

        services
            .AddScoped<LoggingMiddleware>()
            .AddScoped<ExceptionsMiddleware>();

        services
            .SetupMetrics()
            .SetupMqtt(_configuration)
            .SetupApplication()
            .SetupYandex(_configuration);

        services
            //.AddMqttHandler<LogZigbee2MqttEventsHandler>(new QueueConnection("zigbee2mqtt/+", "test"))
            .AddMqttHandler<MqttDevicesControlsMetricsHandler>(new QueueConnection("/devices/+/controls/+", "prometheus"))
            .AddMqttHandler<ParseZigbee2MqttEventsHandler>(new QueueConnection("zigbee2mqtt/+", "zigbee2mqtt_client"))
            ;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger(swaggerOptions => { swaggerOptions.RouteTemplate = "/swagger/{documentName}/swagger.json"; });

        app.UseSwaggerUI(swaggerUiOptions =>
        {
            swaggerUiOptions.RoutePrefix = "swagger";
            swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
            swaggerUiOptions.DisplayRequestDuration();
        });

        app.UseMiddleware<LoggingMiddleware>()
            .UseMiddleware<ExceptionsMiddleware>();

        app.UseRouting()
            .UseAuthentication()
            .UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.AddMetricsPullHost();
        });
    }
}