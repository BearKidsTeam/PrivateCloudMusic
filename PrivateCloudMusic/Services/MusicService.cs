using System;
using System.IO;
using LiteDB;
using Pcm.Entities;
using Pcm.Utils;

namespace Pcm.Services
{
    public class MusicService
    {
        static readonly Lazy<MusicService> lazy = new Lazy<MusicService>(() => new MusicService());
        public static MusicService Instance => lazy.Value;
        
        private FileSystemWatcher fsw;

        private MusicService()
        {
            Collection = DbManager.DB.GetCollection<Music>("music");
            var musicPath = ConfigManager.Get("musicPath", "music");

            var dir = Directory.Exists(musicPath) ? new DirectoryInfo(musicPath) : Directory.CreateDirectory(musicPath);
            
            fsw = new FileSystemWatcher()
            {
                Path = dir.FullName,
                IncludeSubdirectories = true
            };
            fsw.Created += FswOnCreated;
            fsw.Changed += FswOnChanged;
            fsw.Deleted += FswOnDeleted;
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

        public Music Add(string filePath)
        {
            var exist = Collection.FindOne(t => t.FilePath == filePath);
            if (exist != null)
            {
                return exist;
            }
            
            var tfile = TagLib.File.Create(filePath);

            var id = ObjectId.NewObjectId();
            
            var music = new Music
            {
                MusicId = id,
                Title = tfile.Tag.Title,
                Album = tfile.Tag.Album,
                Genres = tfile.Tag.Genres,
                Performers = tfile.Tag.Performers,
                Track = tfile.Tag.Track,
                TrackCount = tfile.Tag.TrackCount,
                Length = (int)tfile.Properties.Duration.TotalSeconds,
                MimeType = tfile.MimeType,
                FilePath = filePath,
                PlayCount = 0,
            };
            
            var bsonId = Collection.Insert(music);

            return Get(id.ToString());
        }
        
    }
}