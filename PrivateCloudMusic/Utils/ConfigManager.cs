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

        public static int GetInt(string key, int defaultVal = 0)
        {
            var strVal = Get(key);
            
            if (string.IsNullOrEmpty(strVal) || !int.TryParse(strVal, out var val))
            {
                return defaultVal;
            }

            return val;
        }
    }
}