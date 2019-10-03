using System.Threading.Tasks;
using Pcm.Proto;
using Pcm.Proxy;

namespace Pcm.Handlers
{
    public class AlbumHandler
    {
        public async Task<StatusCode> Get(Context ctx, Request req, Response resp)
        {
            return await Task.FromResult(StatusCode.Ok);
        }
        
        public async Task<StatusCode> List(Context ctx, Request req, Response resp)
        {
            return await Task.FromResult(StatusCode.Ok);
        }
    }
}