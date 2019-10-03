using System.Collections.Generic;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pcm.Utils;

namespace Pcm.Proxy
{
    public class Context
    {
        public byte[] RawData { get; set; }
        public PayloadType PayloadType { get; set; }
        public HttpContext HttpContext { get; set; }
        public WebSocket WebSocket { get; set; }
        public Dictionary<object, object> Items { get; protected set; } = new Dictionary<object, object>();

        public ILogger Logger { get; protected set; } = LogManager.CreateLogger<Context>();
    }
}
