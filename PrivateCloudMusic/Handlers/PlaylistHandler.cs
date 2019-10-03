using System.Threading.Tasks;
using Pcm.Proto;
using Pcm.Proxy;

namespace Pcm.Handlers
{
    public class PlaylistHandler
    {
        public async Task<StatusCode> List(Context ctx, Request req, Response resp)
        {
            return await Task.FromResult(StatusCode.Ok);
        }
        
        public async Task<StatusCode> Get(Context ctx, Request req, Response resp)
        {
            return await Task.FromResult(StatusCode.Ok);
        }
        
        public async Task<StatusCode> Create(Context ctx, Request req, Response resp)
        {
            return await Task.FromResult(StatusCode.Ok);
        }
        
        public async Task<StatusCode> Rename(Context ctx, Request req, Response resp)
        {
            return await Task.FromResult(StatusCode.Ok);
        }
        
        public async Task<StatusCode> Add(Context ctx, Request req, Response resp)
        {
            return await Task.FromResult(StatusCode.Ok);
        }
        
        public async Task<StatusCode> Remove(Context ctx, Request req, Response resp)
        {
            return await Task.FromResult(StatusCode.Ok);
        }
        
        public async Task<StatusCode> Delete(Context ctx, Request req, Response resp)
        {
            return await Task.FromResult(StatusCode.Ok);
        }
    }
}