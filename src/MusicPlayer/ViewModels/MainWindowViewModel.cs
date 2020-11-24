using MusicPlayer.Infrastructure;
using MusicPlayer.Model;
using MusicPlayer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using TagLib.Mpeg4;

namespace MusicPlayer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public PlayerViewModel PlayerViewModel { get; private set; }
        public LibraryViewModel LibraryViewModel { get; private set; }
        public SettingsViewModel SettingsViewModel { get; private set; }

        public MainWindowViewModel(IMusicPlayer player)
        {
            //IQueueLoader ql = new FileQueueLoader();

            //List<Song> songs = new List<Song>();
            //ql.WalkDirectoryTree(new DirectoryInfo(@"D:\My Music\test"), songs);

            List<Song> songs = new List<Song>();
            if (System.IO.File.Exists(@".\library.json"))
            {
                string songText = System.IO.File.ReadAllText(@".\library.json");
                //JsonSerializerOptions o = new JsonSerializerOptions();
                //o.
                songs = JsonSerializer.Deserialize<List<Song>>(songText);
            }
            else
            {
                songs.Add(new Song { Artist = "No library found. Please go to settings tab to configure your library" });
            }

            PlayerViewModel = new PlayerViewModel(player);
            LibraryViewModel = new LibraryViewModel(songs);
            SettingsViewModel = new SettingsViewModel();
            LibraryViewModel.AddToQueueRequested += AddToPlayerQueue;
            LibraryViewModel.ClearQueueRequested += ClearPlayerQueue;
        }

        private void AddToPlayerQueue()
        {
            List<Song> songsToQueue = new List<Song>();
            if (LibraryViewModel.SelectedAlbum == null)
            {
                foreach (Album a in LibraryViewModel.SelectedArtist.Albums)
                {
                    songsToQueue = songsToQueue.Concat(a.Songs).ToList();
                }
            }
            else
            {
                songsToQueue = LibraryViewModel.SelectedAlbum.Songs.ToList();
            }
            PlayerViewModel.AddToQueue(songsToQueue);
        }

        private void ClearPlayerQueue()
        {
            PlayerViewModel.ClearQueueCommand.Execute(null);
        }
    }
}
