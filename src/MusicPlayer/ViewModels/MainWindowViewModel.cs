using MusicPlayer.Infrastructure;
using MusicPlayer.Model;
using MusicPlayer.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Windows.Input;

namespace MusicPlayer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public PlayerViewModel PlayerViewModel { get; private set; }
        public LibraryViewModel LibraryViewModel { get; private set; }

        public MainWindowViewModel(IMusicPlayer player)
        {
            IQueueLoader ql = new FileQueueLoader();
            SongCollection collection = new SongCollection(ql);

            PlayerViewModel = new PlayerViewModel(player, collection);

            LibraryViewModel = new LibraryViewModel();
            LibraryViewModel.AddToQueueRequested += AddToPlayerQueue;
            LibraryViewModel.ClearQueueRequested += ClearPlayerQueue;
        }

        private void AddToPlayerQueue(string queuePath)
        {
            PlayerViewModel.QueueFilePath = queuePath;
            PlayerViewModel.AddToQueueCommand.Execute(null);
        }

        private void ClearPlayerQueue()
        {
            PlayerViewModel.ClearQueueCommand.Execute(null);
        }
    }
}
