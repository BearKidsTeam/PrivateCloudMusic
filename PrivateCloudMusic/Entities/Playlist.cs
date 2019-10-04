using System.Collections.Generic;
using LiteDB;

namespace Pcm.Entities
{
    public class Playlist
    {
        public ObjectId Id { get; set; }
        
        public string Name { get; set; }
        
        [BsonRef("music")]
        public List<Music> Musics { get; set; }
    }
}