using System.Linq;
using System.Threading.Tasks;
using Pcm.Proto;
using Pcm.Proxy;
using Pcm.Services;

namespace Pcm.Handlers
{
    public class MusicHandler
    {
        public async Task<StatusCode> Get(Context ctx, Request req, Response resp)
        {
            var music = MusicService.Instance.Get(req.Body.GetMusicBody.Id);
            if (music != null)
            {
                resp.Body.GetMusicBody = music;
            }

            return await Task.FromResult(StatusCode.Ok);
        }

        public async Task<StatusCode> List(Context ctx, Request req, Response resp)
        {
            var musics = MusicService.Instance.Collection.FindAll();
            resp.Body.ListMusicBody = new ListMusicResponseBody()
            {
                Musics = { musics.Select(m => m.ToResp()) } 
            };

            return await Task.FromResult(StatusCode.Ok);
        }
        
        public async Task<StatusCode> Delete(Context ctx, Request req, Response resp)
        {
            return await Task.FromResult(StatusCode.Ok);
        }
        
        public async Task<StatusCode> Modify(Context ctx, Request req, Response resp)
        {
            return await Task.FromResult(StatusCode.Ok);
        }
    }
}