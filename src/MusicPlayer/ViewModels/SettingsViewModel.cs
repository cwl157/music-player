using MusicPlayer.Infrastructure;
using MusicPlayer.Model;
using MusicPlayer.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicPlayer.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private ILibraryLoader _loader;
        private DateTime _lastSyncTime;

        public event Action<List<Song>> RefreshLibraryRequested = delegate { };
        public string LibraryFolderPath { get; set; }

        public SettingsViewModel(ILibraryLoader loader)
        {
            if (System.IO.File.Exists(@".\lastSync.json"))
            {
                string lastSyncText = System.IO.File.ReadAllText(@".\lastSync.json");
                var options = new JsonSerializerOptions { Converters = { new MusicPlayer.Infrastructure.TimeSpanConverter() } };
                _lastSyncTime = JsonSerializer.Deserialize<DateTime>(lastSyncText, options);
            }
            else
            {
                _lastSyncTime = DateTime.Now;
            }

            RefreshStatus = "Not Started";
            RefreshLibrary = new CommandHandler(() => RefreshLibraryAction(), () => true);
            _loader = loader;
        }

        private string _refreshStatus;
        public string RefreshStatus
        {
            get { return _refreshStatus; }
            set { SetProperty(ref _refreshStatus, value); }
        }

        public ICommand RefreshLibrary { get; private set; }

        private void RefreshLibraryAction()
        {
            RefreshStatus = "Refreshing...";
            Task.Run(() =>
            {
                List<Song> songs = new List<Song>();
                if (string.IsNullOrEmpty(LibraryFolderPath) || !Directory.Exists(LibraryFolderPath))
                {
                    RefreshStatus = "Invalid directory";
                }
                else
                {
                    _loader.Load(new DirectoryInfo(LibraryFolderPath), songs, _lastSyncTime);

                    var options = new JsonSerializerOptions { Converters = { new TimeSpanConverter() } };
                    string result = JsonSerializer.Serialize(songs, options);
                    File.WriteAllText(@".\library.json", result);
                    _lastSyncTime = DateTime.Now;
                    string lastTime = JsonSerializer.Serialize(_lastSyncTime, options);
                    File.WriteAllText(@".\lastsync.json", lastTime);
                   // DateTime n = new DateTime("Thursday, 10 June 2021 20:33:49")
                    RefreshStatus = "Refresh Complete";
                    RefreshLibraryRequested(songs);
                }
            });            
        }
    }
}
