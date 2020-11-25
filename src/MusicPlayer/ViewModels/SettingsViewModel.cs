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
        public string LibraryFolderPath { get; set; }

        private string _refreshStatus;
        public string RefreshStatus
        {
            get { return _refreshStatus; }
            set { SetProperty(ref _refreshStatus, value); }
        }

        public ICommand RefreshLibrary { get; private set; }

        public SettingsViewModel()
        {
            RefreshStatus = "Not Started";
            RefreshLibrary = new CommandHandler(() => RefreshLibraryAction(), () => true);
        }

        private void RefreshLibraryAction()
        {
            IQueueLoader ql = new FileQueueLoader();

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
                    ql.WalkDirectoryTree(new DirectoryInfo(LibraryFolderPath), songs);

                    var options = new JsonSerializerOptions { Converters = { new TimeSpanConverter() } };
                    //  JsonSerializer.Serialize(myObj, options);
                    string result = JsonSerializer.Serialize(songs, options);
                    File.WriteAllText(@".\library.json", result);
                    RefreshStatus = "Refresh Complete";
                }
            });            
        }
    }
}
