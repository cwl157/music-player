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
        public ICommand AddToQueueClick { get; private set; }
        public ICommand ClearQueueClick { get; private set; }
        public event Action<string> AddToQueueRequested = delegate { };
        public event Action ClearQueueRequested = delegate { };

        public LibraryViewModel()
        {
            AddToQueueClick = new CommandHandler(() => AddToQueueAction(), () => true);
            ClearQueueClick = new CommandHandler(() => ClearQueueAction(), () => true);
        }

        private string _queueFilePath;
        public string QueueFilePath
        {
            get { return _queueFilePath; }
            set { SetProperty(ref _queueFilePath, value); }
        }

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
