using System.Net;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using dotmeer.WbExtensions.Infrastructure.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace dotmeer.WbExtensions.Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                configurationBuilder.Sources.Clear();

                configurationBuilder
                    .AddJsonFile("appsettings.json", false, false)
                    .AddEnvironmentVariables()
                    .Build();
            })
            .SetupLogging()
            .UseDefaultServiceProvider(serviceProviderOptions =>
            {
                serviceProviderOptions.ValidateScopes = true;
                serviceProviderOptions.ValidateOnBuild = true;
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseKestrel(options => { options.Listen(IPAddress.Any, 8000); })
                    .UseStartup<Startup>();
            })
            .Build()
            .RunAsync();
    }
}