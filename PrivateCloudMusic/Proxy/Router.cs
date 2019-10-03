using System.Collections.Generic;
using Pcm.Handlers;
using Pcm.Proto;

namespace Pcm.Proxy
{
    public class Router
    {
        private Dictionary<int, Endpoint> _mapping = new Dictionary<int, Endpoint>();

        public Router()
        {
            initRouter();
        }

        private void initRouter()
        {
            var defaultHandler = new DefaultHandler();
            var music = new MusicHandler();
            var album = new AlbumHandler();
            var playlist = new PlaylistHandler();
            
            register(CMD.NotUsed, defaultHandler.Default); // 0
            register(CMD.GetMusic, music.Get); // 100
            
            register(CMD.GetPlaylist, playlist.Get); // 200
            register(CMD.GetAlbum, album.Get); // 201
            
            register(CMD.ListPlaylist, playlist.List); // 301 
            register(CMD.ListAlbum, album.List); // 302
            
            register(CMD.ActionCreatePlaylist, playlist.Create); // 404
            register(CMD.ActionRenamePlaylist, playlist.Rename); // 405
            register(CMD.ActionAddMusicToPlaylist, playlist.Add); // 406
            register(CMD.ActionRemoveMusicFromPlaylist, playlist.Remove); // 407
            register(CMD.ActionDeletePlaylist, playlist.Delete); // 408
        }

        private void register(CMD code, Endpoint handler)
        {
            _mapping.Add((int)code, handler);
        }

        public Endpoint Process(Context ctx, Request req, Response resp)
        {
            return _mapping.ContainsKey(req.Cmd) ? _mapping[req.Cmd] : new DefaultHandler().Default;
        }
    }
}