using MusicPlayer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MusicPlayer.Model
{
    public class SongCollection
    {
        private readonly IQueueLoader _loader;

        public ObservableCollection<Song> SongList { get; }

        public SongCollection(IQueueLoader ql)
        {
            SongList = new ObservableCollection<Song>();
            _loader = ql;
        }

        public void Load(string filepath)
        {
           var songs = _loader.Load(filepath);

            foreach (Song s in songs)
            {
                SongList.Add(s);
            }
        }

        public double TotalSeconds()
        {
            return SongList.Sum(s => s.Duration.TotalSeconds);
        }
    }
}
