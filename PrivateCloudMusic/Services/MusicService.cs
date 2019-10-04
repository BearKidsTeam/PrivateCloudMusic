using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using Pcm.Entities;
using Pcm.Utils;
using TagLib;

namespace Pcm.Services
{
    public class MusicService
    {
        static readonly Lazy<MusicService> lazy = new Lazy<MusicService>(() => new MusicService());
        public static MusicService Instance => lazy.Value;
        
        private FileSystemWatcher fsw;
        public DirectoryInfo MusicDir { get; set; }

        private MusicService()
        {
            Collection = DbManager.DB.GetCollection<Music>("music");
            var musicPath = ConfigManager.Get("musicPath", "music");

            MusicDir = Directory.Exists(musicPath) ? new DirectoryInfo(musicPath) : Directory.CreateDirectory(musicPath);
            
            fsw = new FileSystemWatcher()
            {
                Path = MusicDir.FullName,
                IncludeSubdirectories = true
            };
            fsw.Created += FswOnCreated;
            fsw.Changed += FswOnChanged;
            fsw.Deleted += FswOnDeleted;
            
            // init when started
            Scan();
        }

        private void FswOnDeleted(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FswOnChanged(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FswOnCreated(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }
        
        public LiteCollection<Music> Collection { get; protected set; }

        public bool EnableWatcher
        {
            get => fsw.EnableRaisingEvents;
            set => fsw.EnableRaisingEvents = value;
        }

        public Music Get(string id)
        {
            var oid = new ObjectId(id);
            return Collection.FindById(oid);
        }

        public IEnumerable<Music> GetAlbum(string name)
        {
            var musics =  Collection.FindAll()
                .Where(t => t.Album == name)
                .OrderBy(m => m.Track);
            
            if (!musics.Any())
            {
                return new List<Music>();
            }
            
            // fix error trackCount on-the-fly, return max track as trackCount
            // currently do not save to database
            // only fix it when group by album
            var maxTrack = musics.Select(m => m.Track).Max();
            return musics.Select(t =>
            {
                if (t.TrackCount == 0)
                {
                    t.TrackCount = maxTrack;
                }
                
                return t;
                
            });
        }

        public Music Add(string filePath)
        {
            try
            {
                var exist = Collection.FindOne(t => t.FilePath == filePath);
                if (exist != null)
                {
                    return exist;
                }
            
                var tfile = TagLib.File.Create(filePath);

                if ((tfile.Properties.MediaTypes & MediaTypes.Audio) != 0)
                {
                    var id = ObjectId.NewObjectId();
            
                    var music = new Music
                    {
                        MusicId = id,
                        Title = tfile.Tag.Title,
                        Album = tfile.Tag.Album ?? "Unknown",
                        Genres = tfile.Tag.Genres,
                        Performers = tfile.Tag.Performers,
                        AlbumArtists = tfile.Tag.AlbumArtists,
                        Track = tfile.Tag.Track,
                        TrackCount = tfile.Tag.TrackCount,
                        PictureCount = tfile.Tag.Pictures.Count(),
                        Length = (int)tfile.Properties.Duration.TotalSeconds,
                        MimeType = tfile.MimeType,
                        FilePath = filePath,
                        PlayCount = 0
                    };
            
                    Collection.Insert(music);
                    return Get(id.ToString());
                }
            }
            catch
            {
                // ignored
            }

            return null;
        }

        public void Scan()
        {
            var files = Directory.GetFiles(MusicDir.FullName, "*.*", SearchOption.AllDirectories)
                .Where(f => Path.GetFileName(f) != ".DS_Store"); // wtf macOS?
            foreach (var file in files)
            {
                Add(file);
            }
        }
        
    }
}