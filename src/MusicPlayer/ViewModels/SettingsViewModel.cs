using MusicPlayer.Infrastructure;
using MusicPlayer.Model;
using MusicPlayer.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace MusicPlayer.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public string LibraryFolderPath { get; set; }

        public ICommand RefreshLibrary { get; private set; }

        public SettingsViewModel()
        {
            RefreshLibrary = new CommandHandler(() => RefreshLibraryAction(), () => true);
        }

        private void RefreshLibraryAction()
        {
            IQueueLoader ql = new FileQueueLoader();

            List<Song> songs = new List<Song>();
            ql.WalkDirectoryTree(new DirectoryInfo(LibraryFolderPath), songs);

            string result  = JsonSerializer.Serialize(songs);
            File.WriteAllText(@".\library.json", result);

        }
    }
}
