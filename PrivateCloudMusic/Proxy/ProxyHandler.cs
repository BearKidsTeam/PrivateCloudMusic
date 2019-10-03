using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Google.Protobuf;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using Pcm.Proto;
using Pcm.Proxy.Middlewares;
using Pcm.Services;

namespace Pcm.Proxy
{
    public class ProxyHandler
    {
        private readonly Dictionary<string, WebSocket> _webSockets = new Dictionary<string, WebSocket>();

        private readonly List<BaseMiddleware> _middlewares = new List<BaseMiddleware>();

        private readonly Dictionary<PayloadType, Parser> _parsers = new Dictionary<PayloadType, Parser>();
        
        private readonly Router _router = new Router();

        public ProxyHandler()
        {
            addMiddlewares();
            addParser();

            // reverse to make sure the req is passed in order
            _middlewares.Reverse();
        }

        private void addMiddlewares()
        {
            // outer
            _middlewares.Add(new Logging());
            _middlewares.Add(new Preset());
            // inner
            // router
        }

        private void addParser()
        {
            _parsers.Add(PayloadType.Json, new JsonParser());
            _parsers.Add(PayloadType.Protobuf, new ProtobufParser());
        }

        public async Task HandleWs(HttpContext context, WebSocket webSocket)
        {
            var guid = new Guid().ToString();
            _webSockets.Add(guid, webSocket);
            try
            {
                var buffer = new byte[1024 * 8];
                while (!webSocket.CloseStatus.HasValue)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (!result.EndOfMessage)
                    {
                        throw new Exception("payload too large");
                    }
                
                    var ctx = new Context()
                    {
                        HttpContext = context,
                        WebSocket = webSocket
                    };
                    var resp = await handle(ctx, buffer);
                    await webSocket.SendAsync(new ArraySegment<byte>(resp), WebSocketMessageType.Binary, true, CancellationToken.None);
                }
            }
            finally
            {
                _webSockets.Remove(guid);
                await webSocket.CloseAsync(WebSocketCloseStatus.Empty, webSocket.CloseStatusDescription, CancellationToken.None);
            }
        }

        public async Task HandleHttp(HttpContext context)
        {
            var ctx = new Context()
            {
                HttpContext = context
            };
            
            var buffer = new byte[1024 * 8];
            var count = await context.Request.Body.ReadAsync(buffer);
            var resp = await handle(ctx, buffer.Take(count).ToArray());
            await context.Response.BodyWriter.WriteAsync(resp);

        }

        private async Task<byte[]> handle(Context ctx, byte[] data)
        {
            ctx.RawData = data;
            
            var (req, type) = parseBody(ctx);
            ctx.PayloadType = type;
            var resp = new Response();

            var endpoint = _router.Process(ctx, req, resp);
            endpoint = _middlewares.Aggregate(endpoint, (current, middleware) => middleware.Create(current));
            await endpoint.Invoke(ctx, req, resp);

            var memBuffer = new MemoryStream();
            await memBuffer.WriteAsync(writeBody(resp, type));
            var buffer = memBuffer.ToArray();

            return buffer;
        }

        private async Task wsBroadcast(Response resp)
        {
            var memBuffer = new MemoryStream();
            resp.WriteTo(memBuffer);
            var buffer = memBuffer.ToArray();
            foreach (var ws in _webSockets.Select(kv => kv.Value).Where(ws => !ws.CloseStatus.HasValue))
            {
                await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }
        
        private Encoding payloadEncoding = Encoding.UTF8;

        private (Request, PayloadType) parseBody(Context ctx)
        {
            foreach (var parser in _parsers)
            {
                 var (req, type) = parser.Value.From(ctx.RawData, payloadEncoding);
                 if (req != null)
                 {
                     return (req, type);
                 }
            }

            throw new NotImplementedException();
        }

        private byte[] writeBody(Response resp, PayloadType type)
        {
            var parser = _parsers[type];
            return parser.To(resp, payloadEncoding);
        }

        public async Task HandleMusic(HttpContext ctx)
        {
            var id = ctx.GetRouteValue("id").ToString();
            
            var music = MusicService.Instance.Get(id);
            if (music == null)
            {
                throw new FileNotFoundException();
            }

            var dir = Path.GetDirectoryName(music.FilePath);
            var fi = new PhysicalFileProvider(dir).GetFileInfo(music.FileName);
            ctx.Response.Headers[HeaderNames.ContentDisposition] = "attachment; filename=" + WebUtility.UrlEncode(music.FileName);
            
            await ctx.Response.SendFileAsync(fi);
        }

        public async Task HandleCover(HttpContext ctx)
        {
            var id = ctx.GetRouteValue("id").ToString();
            var indexObj = ctx.GetRouteValue("index");
            var index = 0;
            if (indexObj != null)
            {
                index = int.Parse(indexObj.ToString());
            }

            var music = MusicService.Instance.Get(id);
            if (music == null)
            {
                throw new FileNotFoundException();
            }
            
            var tfile = TagLib.File.Create(music.FilePath);
            index = Math.Max(0, index);
            index = Math.Min(index, tfile.Tag.Pictures.Count() - 1);
            var img = tfile.Tag.Pictures[index];
            
            ctx.Response.ContentType = img.MimeType;
            await ctx.Response.BodyWriter.WriteAsync(img.Data.Data);
        }
    }
}