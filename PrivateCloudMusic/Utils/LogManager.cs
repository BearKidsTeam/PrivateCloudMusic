using System;
using Microsoft.Extensions.Logging;

namespace Pcm.Utils
{
    public static class LogManager
    {
        private static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole()
                .AddDebug();
        });

        public static ILogger CreateLogger<T>()
        {
            return _loggerFactory.CreateLogger<T>();
        }

        public static ILogger CreateLogger(Type type)
        {
            return _loggerFactory.CreateLogger(type);
        }

        public static ILogger CreateLogger<T>(string msg)
        {
            return _loggerFactory.CreateLogger(typeof(T).FullName + msg);
        }
    }
}