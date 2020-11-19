using MusicPlayer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace MusicPlayer.ViewModels
{
    public class LibraryViewModel : BindableBase
    {
        //private string _name;
        private string _queueFilePath;

        public LibraryViewModel()
        {
            // _name = "LibraryViewModel";
            AddToQueueClick = new CommandHandler(() => AddToQueueAction(), () => true);
            ClearQueueClick = new CommandHandler(() => ClearQueueAction(), () => true);
        }

        public string QueueFilePath
        {
            get { return _queueFilePath; }
            set { SetProperty(ref _queueFilePath, value); }
        }

        public ICommand AddToQueueClick { get; private set; }
        public ICommand ClearQueueClick { get; private set; }
        public event Action<string> AddToQueueRequested = delegate { };
        public event Action ClearQueueRequested = delegate { };

        private void AddToQueueAction()
        {
            Debug.WriteLine(_queueFilePath);
            AddToQueueRequested(_queueFilePath);
        }

        private void ClearQueueAction()
        {
            ClearQueueRequested();
        }
    }
}
