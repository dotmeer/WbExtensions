using System.Text.Json;
using System.Text.Json.Serialization;
using dotmeer.WbExtensions.Application;
using dotmeer.WbExtensions.Application.Jobs;
using dotmeer.WbExtensions.Infrastructure.Metrics;
using dotmeer.WbExtensions.Infrastructure.Mqtt;
using dotmeer.WbExtensions.Service.BackgroundServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace dotmeer.WbExtensions.Service;

internal sealed class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration  configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddControllers()
            .AddControllersAsServices()
            .AddJsonOptions(_ =>
            {
                _.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                _.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        services
            .SetupMetrics()
            .SetupMqtt(_configuration)
            .SetupApplication();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Default", Version = "v1" });
            options.CustomSchemaIds(_ => _.FullName);
            options.UseAllOfToExtendReferenceSchemas();
            options.SupportNonNullableReferenceTypes();
        });

        services.AddRouting(options => { options.AppendTrailingSlash = true; });

        services.AddHostedService<RunJobBackgroundService<ParseZigbee2MqttEventsJob>>();
        services.AddHostedService<RunJobBackgroundService<MqttDevicesControlsMetricsJob>>();
        services.AddHostedService<RunJobBackgroundService<BridgeToYandexJob>>();
        //services.AddHostedService<RunJobBackgroundService<LogZigbee2MqttEventsJob>>();
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
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.AddMetricsPullHost();
        });
    }
}