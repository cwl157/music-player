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
        private PlayerViewModel _playerVm;
        private LibraryViewModel _libraryVm;
        private BindableBase _CurrentViewModel;
        //private CommandHandler NavCommand;
        public ICommand NavigateToPlayer { get; private set; }
        public ICommand NavigateToLibrary { get; private set; }

        public BindableBase CurrentViewModel
        {
            get { return _CurrentViewModel; }
            set { SetProperty(ref _CurrentViewModel, value); }
        }

        public BindableBase PlayerViewModel
        {
            get { return _playerVm; }
        }

        public BindableBase LibraryViewModel
        {
            get { return _libraryVm; }
        }

        public MainWindowViewModel(IMusicPlayer player)
        {
            IQueueLoader ql = new FileQueueLoader();
            SongCollection collection = new SongCollection(ql);

            _playerVm = new PlayerViewModel(player, collection);
            _libraryVm = new LibraryViewModel();
            NavigateToPlayer = new CommandHandler(() => NavToPlayer(), () => true);
            NavigateToLibrary = new CommandHandler(() => NavToLibrary(), () => true);
            _libraryVm.AddToQueueRequested += NavToPlayer;
            _libraryVm.ClearQueueRequested += ClearQueue;

            //CurrentViewModel = _playerVm;
            CurrentViewModel = _libraryVm;
        }

        private void NavToPlayer()
        {
            CurrentViewModel = _playerVm;
        }

        private void NavToPlayer(string queuePath)
        {
            _playerVm.QueueFilePath = queuePath;
            _playerVm.AddToQueueCommand.Execute(null);
       //     CurrentViewModel = _playerVm;
        }

        private void ClearQueue()
        {
            _playerVm.ClearQueueCommand.Execute(null);
        }

        private void NavToLibrary()
        {
            CurrentViewModel = _libraryVm;
        }
    }
}
