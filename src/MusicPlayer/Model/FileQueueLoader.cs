using MusicPlayer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace MusicPlayer.Model
{
    public class FileQueueLoader : IQueueLoader
    {
        public FileQueueLoader()
        {
        }

        public List<Song> Load(string filepath)
        {
            DirectoryInfo dir = new DirectoryInfo(filepath);
            List<Song> songs = new List<Song>();
            foreach (FileInfo f in dir.GetFiles())
            {
                if (f.Extension.ToLower() == ".mp3")
                {
                    var tfile = TagLib.File.Create(f.FullName);
                    Song s = new Song();
                    s.Artist = tfile.Tag.FirstPerformer;
                    s.Album = tfile.Tag.Album;
                    s.Title = tfile.Tag.Title;
                    s.TrackNumber = (int)tfile.Tag.Track;
                    s.Duration = tfile.Properties.Duration;
                    s.FilePath = f.FullName;
                    s.Lyrics = tfile.Tag.Lyrics;
                    if (tfile.Tag.Pictures.Length > 0)
                    {
                        s.AlbumArt = LoadImage(tfile.Tag.Pictures[0].Data.Data);
                    }
                    s.Year = tfile.Tag.Year.ToString();
                    songs.Add(s);
                }
            }

            songs = songs.OrderBy(s => s.TrackNumber).ToList();

            return songs;
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}
