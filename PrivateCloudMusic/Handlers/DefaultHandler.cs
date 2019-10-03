using System.Threading.Tasks;
using Pcm.Proto;
using Pcm.Proxy;

namespace Pcm.Handlers
{
    public class DefaultHandler 
    {
        public async Task<StatusCode> Default(Context ctx, Request req, Response resp)
        {
            return await Task.FromResult(StatusCode.Ok);
        }
    }
}