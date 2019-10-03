using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Pcm.Utils;

namespace Pcm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigManager.Config = new ConfigurationBuilder()
                .AddJsonFile("config.json", true) //this is not needed, but could be useful
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
