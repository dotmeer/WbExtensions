using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using WbExtensions.Application;
using WbExtensions.Application.Handlers.Mqtt;
using WbExtensions.Domain.Mqtt;
using WbExtensions.Infrastructure;
using WbExtensions.Infrastructure.Json;
using WbExtensions.Infrastructure.Metrics;
using WbExtensions.Service.BackgroundServices;
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
            options.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
        });

        services.AddRouting(options => { options.AppendTrailingSlash = true; });
        
        services
            .AddScoped<LoggingMiddleware>()
            .AddScoped<ExceptionsMiddleware>();

        services
            .SetupInfrastructure(_configuration)
            .SetupApplication();

        services
            .AddHostedService<InitializationBackgroundService>()
            //.AddMqttHandler<LogZigbee2MqttEventsHandler>(new QueueConnection("zigbee2mqtt/+", "test"))
            //.AddMqttHandler<ParseZigbee2MqttEventsHandler>(new QueueConnection("zigbee2mqtt/+", "zigbee2mqtt_client"))
            .AddMqttHandler<SubscribeDevicesToMqttHandler>(new QueueConnection("/devices/+/controls/+", "virtual_devices_subscription"))
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

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.AddMetricsPullHost();
        });
    }
}