using System;
using Pcm.Proto;

namespace Pcm.Proxy.Middlewares
{
    public class Preset : BaseMiddleware
    {
        public override Endpoint Create(Endpoint func)
        {
            return async (ctx, req, resp) =>
            {
                resp.Cmd = req.Cmd;
                resp.SequenceId = req.SequenceId;
                resp.Body = new ResponseBody();

                var code = StatusCode.Ok;

                try
                {
                    code = await func(ctx, req, resp);
                }
                catch (Exception ex)
                {
                    resp.StatusCode = (int) code;
                    resp.ErrorDesc = ex.Message;
                    throw;
                }
                
                resp.StatusCode = (int)code;
                resp.ErrorDesc = "OK";

                if (ctx.WebSocket == null)
                {
                    ctx.HttpContext.Response.ContentType = ctx.PayloadType switch
                    {
                        PayloadType.Json => "application/json",
                        PayloadType.Protobuf => "application/x-protobuf",
                        _ => ctx.HttpContext.Response.ContentType
                    };
                }
                
                return code;
            };
        }
    }
}