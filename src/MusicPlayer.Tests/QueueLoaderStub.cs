using MusicPlayer.Model;
using MusicPlayer.Services;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MusicPlayer.Tests
{
    internal class QueueLoaderStub : IQueueLoader
    {
        public List<Song> Load(string path)
        {
            var tmp = new List<Song>();
            tmp.Add(new Song()
            {
                Artist = "Test",
                Title = "Title",
                Album = "Test Album",
                TrackNumber = 1,
                Duration = TimeSpan.FromSeconds(10),
                FilePath = @"C:\tmp\1",
                Year = "2018"
            });
            tmp.Add(new Song()
            {
                Artist = "Test",
                Title = "Title Two",
                Album = "Test Album",
                TrackNumber = 2,
                Duration = TimeSpan.FromSeconds(20),
                FilePath = @"C:\tmp\2",
                Year = "2019"
            });

            return tmp;
        }

        public void WalkDirectoryTree(DirectoryInfo root, List<Song> result)
        {
            throw new NotImplementedException();
        }
    }
}
