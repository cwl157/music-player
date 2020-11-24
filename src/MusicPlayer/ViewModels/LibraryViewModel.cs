using MusicPlayer.Infrastructure;
using MusicPlayer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MusicPlayer.ViewModels
{
    public class LibraryViewModel : BindableBase
    {
        public ICommand AddToQueueClick { get; private set; }
        public ICommand ClearQueueClick { get; private set; }
        public event Action AddToQueueRequested = delegate { };
        public event Action ClearQueueRequested = delegate { };

        public ObservableCollection<Artist> Artists { get; private set; }

        private ObservableCollection<Album> _albums;
        public ObservableCollection<Album> Albums
        {
            get { return _albums; }
            set { SetProperty(ref _albums, value); }
        }

        private Artist _selectedArtist;
        public Artist SelectedArtist
        {
            get
            {
                return _selectedArtist;
            }
            set
            {
                SetProperty(ref _selectedArtist, value);
                Albums = Artists.FirstOrDefault(a => a.Name == SelectedArtist.Name).Albums;
            }
        }

        private Album _selectedAlbum;
        public Album SelectedAlbum { get { return _selectedAlbum; } set { SetProperty(ref _selectedAlbum, value); } }

        private string _artistHeader;
        public string ArtistHeader
        {
            get { return _artistHeader; }
            set { SetProperty(ref _artistHeader, value); }
        }

        private string _albumHeader;
        public string AlbumHeader
        {
            get { return _albumHeader; }
            set { SetProperty(ref _albumHeader, value); }
        }

        private string _trackHeader;
        public string TrackHeader
        {
            get { return _trackHeader; }
            set { SetProperty(ref _trackHeader, value); }
        }

        public LibraryViewModel(IEnumerable<Song> songs)
        {
            AddToQueueClick = new CommandHandler(() => AddToQueueAction(), () => true);
            ClearQueueClick = new CommandHandler(() => ClearQueueAction(), () => true);
            Artists = new ObservableCollection<Artist>();
            Albums = new ObservableCollection<Album>();

            _selectedArtist = new Artist();
            _selectedAlbum = new Album();
          
            IEnumerable<string> aNames = songs.Select(s => s.Artist).Distinct();
            foreach (string s in aNames)
            {
                var newArtist = new Artist() { Name = s, Albums = new ObservableCollection<Album>() };
                var tmp = songs.Where(a => a.Artist == s);
                var al = tmp.Select(aa => aa.Album).Distinct();
                foreach (string album in al)
                {
                    var ss = songs.Where(t => t.Artist == s && t.Album == album);
                    
                    var newAlbum = new Album();
                    newAlbum.Songs = new ObservableCollection<Song>();
                    newAlbum.Title = album;
                    foreach (Song ts in ss)
                    {
                        newAlbum.Songs.Add(ts);
                    }
                    
                    int outYear = 0;
                    int.TryParse(ss.First().Year, out outYear);
                    newAlbum.Year = outYear;
                    newAlbum.Duration = "00:00";
                    if (newAlbum.Songs.Count > 0)
                    {
                        double totalSeconds = newAlbum.Songs.Sum(s => s.Duration.TotalSeconds);
                        string format = "hh\\:mm\\:ss";
                        if (totalSeconds < 3600)
                        {
                            format = "mm\\:ss";
                        }
                        TimeSpan totalDuration = TimeSpan.FromSeconds(totalSeconds);
                        newAlbum.Duration = totalDuration.ToString(format);
                    }
                    newArtist.Albums.Add(newAlbum);
                }
                Artists.Add(newArtist);
            }
            ArtistHeader = "Artists (" + Artists.Count + ")";
            AlbumHeader = "Albums (" + Artists.Sum(a => a.AlbumCount) + ")";
            TrackHeader = "Tracks (" + Artists.Sum(a => a.TrackCount) + ")";
        }

        //private string _queueFilePath;
        //public string QueueFilePath
        //{
        //    get { return _queueFilePath; }
        //    set { SetProperty(ref _queueFilePath, value); }
        //}

        private void AddToQueueAction()
        {
            AddToQueueRequested();
        }

        private void ClearQueueAction()
        {
            ClearQueueRequested();
        }
    }
}
