using System;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace MusicPlayer.Model
{
    public class Song
    {
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public int TrackNumber { get; set; }
        public TimeSpan Duration { get; set; }
        public string DisplayDuration
        { get
            {
                return Duration.ToString("mm\\:ss");
            }
        }
        public string Lyrics { get; set; }
        public string Year { get; set; } 
        public string Comment { get; set; }

        public DateTime DateAdded { get; set; }
    }
}
