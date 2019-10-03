using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pcm.Entities;
using Pcm.Proto;
using Pcm.Proxy;
using Pcm.Services;
using Pcm.Utils;
using System;

namespace Pcm.Handlers
{
    public class CategoryHandler
    {
        public async Task<StatusCode> ListByAlbum(Context ctx, Request req, Response resp)
        {
            var musics = MusicService.Instance.Collection.FindAll()
                .Select(t => t.ToResp())
                .GroupBy(m => m.Album)
                .ToDictionary(m => m.Key, m => new ListMusicResponseBody() { Musics = { m.ToList() }});
            
            resp.Body.ListByAlbumBody = new ListByAlbumResponseBody()
            {
                Albums = { musics }
            };
            return await Task.FromResult(StatusCode.Ok);
        }
        
        
        public async Task<StatusCode> ListByPerformer(Context ctx, Request req, Response resp)
        {
            var musics = MusicService.Instance.Collection.FindAll()
            // extend to musics with single performer
            .SelectMany(m => m.Performers.Select(performer => new Music()
            {
                MusicId = m.MusicId,
                Title = m.Title,
                Album = m.Album,
                Genres = m.Genres,
                Performers = new[] {performer},
                Track = m.Track,
                TrackCount = m.TrackCount,
                PictureCount = m.PictureCount,
                FilePath = m.FilePath,
                PlayCount = m.PlayCount,
                Length = m.Length,
                MimeType = m.MimeType
            }))
            .GroupBy(t => t.Performers[0])
            .ToDictionary(t => t.Key, t => new List<GetMusicResponseBody>(t.Select(m => m.MusicId)
                .Distinct()
                .Select(m => MusicService.Instance.Get(m.ToString()))
                .Select(m => m.ToResp())))
            .ToDictionary(t => t.Key, t => new ListMusicResponseBody() { Musics = { t.Value }});
            
            resp.Body.ListByPerformerBody = new ListByPerformerResponseBody()
            {
                Performers = { musics }
            };
            return await Task.FromResult(StatusCode.Ok);
        }
        
        public async Task<StatusCode> Search(Context ctx, Request req, Response resp)
        {
            var keywords = req.Body.ListSearchBody.Keyword.ToLower().RemoveSpecialCharacters().Split(" ");
            var searchThreshold = ConfigManager.GetInt("searchThreshold", 2);

            var musics = MusicService.Instance.Collection.FindAll() 
            // extend to musics with single performer
            .SelectMany(m => m.Performers.Select(performer => new Music()
            {
                MusicId = m.MusicId,
                Title = m.Title,
                Album = m.Album ?? string.Empty,
                Genres = m.Genres,
                Performers = new[] {performer},
                Track = m.Track,
                TrackCount = m.TrackCount,
                PictureCount = m.PictureCount,
                FilePath = m.FilePath,
                PlayCount = m.PlayCount,
                Length = m.Length,
                MimeType = m.MimeType
            }))
            .Where(t => t.Album.ToLower().Distance(keywords) <= searchThreshold ||
                        t.Title.ToLower().Distance(keywords) <= searchThreshold ||
                        t.Performers[0].ToLower().Distance(keywords) <= searchThreshold ||
                        t.FileName.ToLower().Distance(keywords) <= searchThreshold)
            .Select(t => t.MusicId)
            .Distinct()
            .Select(t => MusicService.Instance.Get(t.ToString()))
            .Select(t => t.ToResp());

            resp.Body.ListSearchBody = new ListSearchResponseBody()
            {
                Musics = { musics }
            };
            return await Task.FromResult(StatusCode.Ok);
        }
    }
}