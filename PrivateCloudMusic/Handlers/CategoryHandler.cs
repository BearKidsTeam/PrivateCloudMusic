using System.Linq;
using System.Threading.Tasks;
using Pcm.Proto;
using Pcm.Proxy;
using Pcm.Services;

namespace Pcm.Handlers
{
    public class CategoryHandler
    {
        public async Task<StatusCode> Get(Context ctx, Request req, Response resp)
        {
            return await Task.FromResult(StatusCode.Ok);
        }
        
        public async Task<StatusCode> List(Context ctx, Request req, Response resp)
        {
            var musics = MusicService.Instance.Collection.FindAll()
                .Select(t => t.ToResp())
                .GroupBy(m => m.Album)
                .ToDictionary(m => m.Key, m => new ListMusicResponseBody() { Musics = { m.ToList() }});
            
            resp.Body.ListByAlbum = new ListByAlbumResponseBody()
            {
                Albums = { musics }
            };
            return await Task.FromResult(StatusCode.Ok);
        }
    }
}