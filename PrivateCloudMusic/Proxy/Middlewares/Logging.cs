using System;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Pcm.Proto;

namespace Pcm.Proxy.Middlewares
{
    public class Logging : BaseMiddleware
    {
        public static readonly string K_LOGID = "logid";

        public override Endpoint Create(Endpoint next)
        {
            return async (ctx, req, resp) =>
            {
                var logid = Guid.NewGuid();
                ctx.Items.Add(K_LOGID, logid);
                resp.LogId = logid.ToString();

                Exception err = null;
                var code = StatusCode.Ok;

                try
                {
                    code = await next(ctx, req, resp);
                }
                catch (Exception ex)
                {
                    err = ex;
                }

                

                var jsonReq = JsonFormatter.Default.Format(req);
                var jsonResp = JsonFormatter.Default.Format(resp);

                if (err != null)
                {
                    ctx.Logger.LogError("err={0}, req={1}, resp={2}", jsonReq, jsonResp);
                }
                else
                {
                    ctx.Logger.LogInformation("req={0}, resp={1}", jsonReq, jsonResp);
                }

                return code;
            };
        }
    }
}