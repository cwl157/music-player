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
        private List<Song> _songs;

        public MainWindowViewModel(IMusicPlayer player)
        {
            _songs = new List<Song>();
            try
            {
                if (System.IO.File.Exists(@".\library.json"))
                {
                    string songText = System.IO.File.ReadAllText(@".\library.json");
                    var options = new JsonSerializerOptions { Converters = { new MusicPlayer.Infrastructure.TimeSpanConverter() } };
                    _songs = JsonSerializer.Deserialize<List<Song>>(songText, options);
                }
                else
                {
                    _songs.Add(new Song { Artist = "No library found. Please go to settings tab to configure your library" });
                }  
            }
            catch (Exception e)
            {
                _songs.Add(new Song { Artist = "Error loading library. Please go to settings tab and try refreshing the library", Album = e.Message });
            }

            PlayerViewModel = new PlayerViewModel(player);
            LibraryViewModel = new LibraryViewModel(_songs);
            SettingsViewModel = new SettingsViewModel();
            LibraryViewModel.AddToQueueRequested += AddToPlayerQueue;
            LibraryViewModel.ClearQueueRequested += ClearPlayerQueue;
        }

        private void AddToPlayerQueue()
        {
            //List<Song> songsToQueue = new List<Song>();
            IEnumerable<Song> songsToQueue = _songs.Where(s => s.Album == LibraryViewModel.SelectedAlbum.Title);
            //if (LibraryViewModel.SelectedAlbum == null)
            //{
            //    foreach (Album a in LibraryViewModel.SelectedArtist.Albums)
            //    {
            //        songsToQueue = songsToQueue.Concat(a.Songs).ToList();
            //    }
            //}
            //else
            //{
            //    songsToQueue = LibraryViewModel.SelectedAlbum.Songs.ToList();
            //}
            PlayerViewModel.AddToQueue(songsToQueue);
        }

        private void ClearPlayerQueue()
        {
            PlayerViewModel.ClearQueueCommand.Execute(null);
        }
    }
}
