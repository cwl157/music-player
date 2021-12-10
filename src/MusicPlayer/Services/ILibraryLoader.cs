using MusicPlayer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicPlayer.Services
{
    public interface ILibraryLoader
    {
        //List<Song> Load(string path);
        void Load(System.IO.DirectoryInfo root, List<Song> result, DateTime lastSyncTime);
        void RefreshSongs(System.IO.DirectoryInfo root, List<Song> _songs);
    }
}
