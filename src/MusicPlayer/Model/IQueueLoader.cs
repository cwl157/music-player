using MusicPlayer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicPlayer.Model
{
    public interface IQueueLoader
    {
        List<Song> Load(string path);
    }
}
