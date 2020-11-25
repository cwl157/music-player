using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MusicPlayer.Model
{
    public class Album
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public int TotalTracks { get; set; }

        public string Duration { get; set; }

      //  public ObservableCollection<Song> Songs { get; set; }
        public List<string> ArtistNames { get; set; }
    }
}
