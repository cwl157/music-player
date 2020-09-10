using System;
using System.Collections.Generic;
using System.Text;
using TagLib;

namespace MusicPlayer.Models
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
        public IPicture AlbumArt { get; set; }
    }
}
