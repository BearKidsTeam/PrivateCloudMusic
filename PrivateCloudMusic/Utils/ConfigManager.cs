using Microsoft.Extensions.Configuration;

namespace Pcm.Utils
{
    public class ConfigManager
    {
        public static IConfiguration Config;

        public static string Get(string key, string defaultVal = "")
        { 
            return Config[key] ?? defaultVal;
        }
    }
}