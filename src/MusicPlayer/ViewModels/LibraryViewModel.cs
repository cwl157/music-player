using MusicPlayer.Infrastructure;
using MusicPlayer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
        //private ArtistCollection _artists;
        //private AlbumCollection _albums;

        //private ObservableCollection<Artist> _artists;
        public ObservableCollection<Artist> Artists { get; private set; }
        public ObservableCollection<Album> Albums { get; private set; }

        public LibraryViewModel(SongCollection songs)
        {
            AddToQueueClick = new CommandHandler(() => AddToQueueAction(), () => true);
            ClearQueueClick = new CommandHandler(() => ClearQueueAction(), () => true);
            Artists = new ObservableCollection<Artist>();
           // Dictionary<string, List<Album>> artists = new Dictionary<string, List<Albums>>();
            foreach (Song s in songs.SongList)
            {
                //if (artists.ContainsKey(s.Artist))
                //{
                //    artists[s.Artist].Add(new Album() { Title })
                //}
                Artists.Add(new Artist() { Name = s.Artist, AlbumCount = 1, TrackCount = 3 });
                
            }

            // _artists = new ObservableCollection<Artist>() { new Artist() { Name = "Abrasion", AlbumCount= 1, TrackCount= 3 } };
            //_artists = a;
            //_artists.Load("");
            //_albums = aa;
            //_albums.Load("");
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
