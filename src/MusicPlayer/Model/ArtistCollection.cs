using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MusicPlayer.Model
{
    public class ArtistCollection
    {
        public ObservableCollection<Artist> ArtistList { get; }

        public ArtistCollection()
        {
            ArtistList = new ObservableCollection<Artist>();
        }

        public void Load(string filepath)
        {
            //var  = _loader.Load(filepath);

            //foreach (Song s in songs)
           // {
                ArtistList.Add(new Artist() { Name = "Abrasion", AlbumCount = 1, TrackCount = 3 });
            ArtistList.Add(new Artist() { Name = "Alkaline Trio", AlbumCount = 21, TrackCount = 189 });
           // }
        }
    }
}
