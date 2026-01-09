using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using WbExtensions.Infrastructure.Logging;

namespace WbExtensions.Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.Sources.Clear();

        builder.Configuration
            .AddJsonFile("appsettings.json", false, false)
            .AddJsonFile("appsettings.local.json", true, false)
            .AddJsonFile("appsettings.schema.json", true, false)
            .AddEnvironmentVariables()
            .Build();

        builder.SetupLogging();

        builder.Host.UseDefaultServiceProvider(serviceProviderOptions =>
        {
            serviceProviderOptions.ValidateScopes = true;
            serviceProviderOptions.ValidateOnBuild = true;
        });

        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);

        var app = builder.Build();
        startup.Configure(app, app.Environment);

        await app.RunAsync();
    }
}