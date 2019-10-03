using System.IO;
using LiteDB;

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
        
        public uint Track { get; set; }
        public uint TrackCount { get; set; }
        
        public string FileName => Path.GetFileName(FilePath);
        public string FilePath { get; set; }
        public int PlayCount { get; set; }
        public string MimeType { get; set; }
        public long Length { get; set; }
    }
}