using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MusicPlayer.Model
{
    public class AlbumCollection
    {
        public ObservableCollection<Album> AlbumList { get; }

        public AlbumCollection()
        {
            AlbumList = new ObservableCollection<Album>();
        }

        public void Load(string filepath)
        {
            //var  = _loader.Load(filepath);

            //foreach (Song s in songs)
            // {
            AlbumList.Add(new Album() { Title = "Demonstration", Year = 2020, TotalTracks = 3, Duration="01:00" });
           // AlbumList.Add(new Artist() { Name = "Alkaline Trio", AlbumCount = 21, TrackCount = 189 });
            // }
        }
    }
}
