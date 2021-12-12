using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media.Imaging;

namespace MusicPlayer.Model
{
    public class Album
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public int TotalTracks { get; set; }

        public string Duration { get; set; }
        public string DisplayTitle { get { return Title + " - " + Year; } }
        public string DisplayInfo { get { return "Tracks: " + TotalTracks + " Duration: " + Duration; } }
        public BitmapImage AlbumArt { get; set; }

        public List<string> ArtistNames { get; set; }
        public string DisplayArtist { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
