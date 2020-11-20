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

            var songs = ql.Load(@"D:\My Music\Full Albums\Abrasion\Demonstration");

            foreach (Song s in songs)
            {
                collection.SongList.Add(s);
            }

            songs = ql.Load(@"D:\My Music\Full Albums\A Vulture Wake\Fall Prey");

            foreach (Song s in songs)
            {
                collection.SongList.Add(s);
            }

            songs = ql.Load(@"D:\My Music\Full Albums\A Vulture Wake\The Appropriate Level of Outrage");

            foreach (Song s in songs)
            {
                collection.SongList.Add(s);
            }

            PlayerViewModel = new PlayerViewModel(player, collection);

            LibraryViewModel = new LibraryViewModel(collection);
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
