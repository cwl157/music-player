using MusicPlayer.Infrastructure;
using MusicPlayer.Model;
using MusicPlayer.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public event Action<List<Song>> RefreshLibraryRequested = delegate { };
        public string LibraryFolderPath { get; set; }

        public SettingsViewModel(ILibraryLoader loader)
        {
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
           // ILibraryLoader fl = new FileLibraryLoader();

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
                    _loader.Load(new DirectoryInfo(LibraryFolderPath), songs);

                    var options = new JsonSerializerOptions { Converters = { new TimeSpanConverter() } };
                    string result = JsonSerializer.Serialize(songs, options);
                    File.WriteAllText(@".\library.json", result);
                    RefreshStatus = "Refresh Complete";
                    RefreshLibraryRequested(songs);
                }
            });            
        }
    }
}
