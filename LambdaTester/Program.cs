using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LambdaTester
{
    class Program
    {
        //We need this in ConsoleApp.cs since we can't DI into a static class
        public static ServiceProvider ServiceProviderProvider;
        private static async Task Main(string[] args)
        {
            // Create service collection and configure our services
            var services = ConfigureServices();
            // Generate a provider
            var serviceProvider = services.BuildServiceProvider();
            ServiceProviderProvider = serviceProvider;

            // Kick off our actual code
            await ConsoleApp.Run();
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();


            // Set up the objects we need to get to configuration settings
            var config = LoadConfiguration();
            var apiSettings = config.GetSection("ApiSettings").Get<ApiSettings>();
            // Add the config to our DI container for later use
            services.AddSingleton(config);
            services.AddSingleton(apiSettings);

            // IMPORTANT! Register our application entry point
            services.AddTransient<ConsoleApp>();
            return services;
        }

        private static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables();
            return builder.Build();
        }
    }
}
