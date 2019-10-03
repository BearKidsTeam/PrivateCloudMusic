using System.Threading.Tasks;
using Pcm.Proto;

namespace Pcm.Proxy
{
    public delegate Task<StatusCode> Endpoint(Context ctx, Request req, Response resp);
}