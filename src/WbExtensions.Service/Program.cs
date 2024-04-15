using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using WbExtensions.Infrastructure.Logging;

namespace WbExtensions.Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        await CreateWebHostBuilder(args)
            .Build()
            .RunAsync();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                configurationBuilder.Sources.Clear();

                configurationBuilder
                    .AddJsonFile("appsettings.json", false, false)
                    .AddJsonFile("appsettings.local.json", true, false)
                    .AddEnvironmentVariables()
                    .Build();
            })
            .SetupLogging()
            .UseDefaultServiceProvider(serviceProviderOptions =>
            {
                serviceProviderOptions.ValidateScopes = true;
                serviceProviderOptions.ValidateOnBuild = true;
            })
            .UseStartup<Startup>();
}