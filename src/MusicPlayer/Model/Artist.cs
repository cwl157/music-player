using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MusicPlayer.Model
{
    public class Artist
    {
        public string Name { get; set; }
        public int AlbumCount { get { return Albums.Count; } }
        public int TrackCount { get { return Albums.Sum(a => a.TotalTracks); } }

        public ObservableCollection<Album> Albums { get; set; }
    }
}
