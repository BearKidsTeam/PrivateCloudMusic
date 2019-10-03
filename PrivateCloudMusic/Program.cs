using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Pcm.Services;
using Pcm.Utils;

namespace Pcm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigManager.Config = new ConfigurationBuilder()
                .AddJsonFile("config.json", optional: true) //this is not needed, but could be useful
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            
            var m = MusicService.Instance.Add(@"/Users/xiaozijin/Downloads/03.オードリー.mp3");

            
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
