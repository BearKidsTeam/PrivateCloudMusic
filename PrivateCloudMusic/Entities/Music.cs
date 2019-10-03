using System.IO;
using LiteDB;
using Pcm.Proto;

namespace Pcm.Entities
{
    public class Music
    {
        [BsonId]
        public ObjectId MusicId { get; set; }
        
        public string Title { get; set; }
        public string Album { get; set; }
        public string[] Genres { get; set; }
        public string[] Performers { get; set; }
        public string[] AlbumArtists { get; set; }
        
        public uint Track { get; set; }
        public uint TrackCount { get; set; }
        public int PictureCount { get; set; }
        
        public string FileName => Path.GetFileName(FilePath);
        public string FilePath { get; set; }
        public int PlayCount { get; set; }
        public string MimeType { get; set; }
        public long Length { get; set; }
        
        public GetMusicResponseBody ToResp() => new GetMusicResponseBody()
        {
            Id = MusicId.ToString(),
            Title = Title,
            Album = Album ?? string.Empty,
            Genres = { Genres },
            Performers = { Performers },
            AlbumArtists = { AlbumArtists },
            Track = Track,
            TrackCount = TrackCount != 0 ? TrackCount : Track,
            PictureCount = PictureCount,
            FileName = FileName,
            PlayCount = PlayCount,
            Length = Length,
            MimeType = MimeType,
            CreatedAt = MusicId.Timestamp
        };

        public static implicit operator GetMusicResponseBody(Music music) => music.ToResp();
    }
}