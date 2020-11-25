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

        private List<Album> _allAlbums;
        private List<Artist> _allArtists;

        public ObservableCollection<Artist> Artists { get; private set; }

        private ObservableCollection<Album> _albums;
        public ObservableCollection<Album> Albums
        {
            get
            {
                return _albums;

            }
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
                _albums.Clear();
                IEnumerable<Album> r = Enumerable.Empty<Album>();
                //if (_selectedArtist.Name == "Show All")
                if (_selectedIndex == 0)
                {
                    r = _allAlbums;
                }
                else
                {
                    r = _allAlbums.Where(a => a.ArtistNames.Contains(SelectedArtist.Name));
                }
                
                foreach (Album aa in r)
                {
                    _albums.Add(aa);
                }
            }
        }

        private int _selectedIndex;

        public int SelectedIndex { get { return _selectedIndex; } set { SetProperty(ref _selectedIndex, value); } }

        private Album _selectedAlbum;
        public Album SelectedAlbum { get { return _selectedAlbum; } set { SetProperty(ref _selectedAlbum, value); } }

        //private string _artistHeader;
        //public string ArtistHeader
        //{
        //    get { return _artistHeader; }
        //    set { SetProperty(ref _artistHeader, value); }
        //}

        //private string _albumHeader;
        //public string AlbumHeader
        //{
        //    get { return _albumHeader; }
        //    set { SetProperty(ref _albumHeader, value); }
        //}

        //private string _trackHeader;
        //public string TrackHeader
        //{
        //    get { return _trackHeader; }
        //    set { SetProperty(ref _trackHeader, value); }
        //}

        public LibraryViewModel(IEnumerable<Song> songs)
        {
            AddToQueueClick = new CommandHandler(() => AddToQueueAction(), () => true);
            ClearQueueClick = new CommandHandler(() => ClearQueueAction(), () => true);

            _allArtists = new List<Artist>();
            _allAlbums = new List<Album>();

            _selectedArtist = new Artist();
            _selectedAlbum = new Album();
          
            IEnumerable<string> artistNames = songs.Select(s => s.Artist).Distinct();
            artistNames = artistNames.OrderBy(s => s);
            foreach (string s in artistNames)
            {
                var newArtist = new Artist() { Name = s };
                var artistSongs = songs.Where(a => a.Artist == s);
                var artistAlbums = artistSongs.Select(aa => new { title = aa.Album, year = aa.Year }).Distinct();
                newArtist.AlbumCount = artistAlbums.Count();
                newArtist.TrackCount = artistSongs.Count();

                //var al = tmp.Select(aa => new { title = aa.Album, year = aa.Year }).Distinct();
                //al = al.OrderByDescending(a => a.year);
                //foreach (dynamic album in al)
                //{
                //    var ss = songs.Where(t => t.Artist == s && t.Album == album.title);

                //    var newAlbum = new Album();
                //    newAlbum.Songs = new ObservableCollection<Song>();
                //    newAlbum.Title = album.title;

                //    foreach (Song ts in ss)
                //    {
                //        newAlbum.Songs.Add(ts);
                //    }

                //    int outYear = 0;
                //    int.TryParse(ss.First().Year, out outYear);
                //    newAlbum.Year = outYear;
                //    newAlbum.Duration = "00:00";
                //    if (newAlbum.Songs.Count > 0)
                //    {
                //        double totalSeconds = newAlbum.Songs.Sum(s => s.Duration.TotalSeconds);
                //        string format = "hh\\:mm\\:ss";
                //        if (totalSeconds < 3600)
                //        {
                //            format = "mm\\:ss";
                //        }
                //        TimeSpan totalDuration = TimeSpan.FromSeconds(totalSeconds);
                //        newAlbum.Duration = totalDuration.ToString(format);
                //    }
                //    newArtist.Albums.Add(newAlbum);
                //   // Albums.Add(newAlbum);
                //}
                _allArtists.Add(newArtist);
            }

            _allArtists.Insert(0, new Artist { Name = "Show All (" + _allArtists.Count + ")", AlbumCount = _allArtists.Sum(a => a.AlbumCount), TrackCount = _allArtists.Sum(a => a.TrackCount)  });
            Artists = new ObservableCollection<Artist>(_allArtists);

            var albumNames = songs.Select(s => new { Title = s.Album, Year = s.Year }).Distinct();
            albumNames = albumNames.OrderByDescending(a => a.Year);
            foreach (var name in albumNames)
            {
                var ss = songs.Where(t => t.Album == name.Title && t.Year == name.Year);
                var newAlbum = new Album();
               // newAlbum.Songs = new ObservableCollection<Song>();
                newAlbum.Title = name.Title;
                newAlbum.TotalTracks = ss.Count();
                int outYear = 0;
                int.TryParse(ss.First().Year, out outYear);
                newAlbum.Year = outYear;
                newAlbum.ArtistNames = new List<string>();
                IEnumerable<string> artists = ss.Select(s => s.Artist).Distinct();
                foreach (string ns in artists)
                {
                    newAlbum.ArtistNames.Add(ns);
                }
                //newAlbum.ArtistNames.Add(ss.FirstOrDefault().Artist)


                //foreach (Song ts in ss)
                //{
                //    newAlbum.Songs.Add(ts);
                //}

                //int outYear = 0;
                //int.TryParse(ss.First().Year, out outYear);
                //newAlbum.Year = outYear;
                newAlbum.Duration = "00:00";
                if (ss.Count() > 0)
                {
                    double totalSeconds = ss.Sum(s => s.Duration.TotalSeconds);
                    string format = "hh\\:mm\\:ss";
                    if (totalSeconds < 3600)
                    {
                        format = "mm\\:ss";
                    }
                    TimeSpan totalDuration = TimeSpan.FromSeconds(totalSeconds);
                    newAlbum.Duration = totalDuration.ToString(format);
                }
                _allAlbums.Add(newAlbum);
            }
            Albums = new ObservableCollection<Album>(_allAlbums);

            
            //Albums.Insert(0, "Show All");

            //var allAlbums = songs.Select(aa => new { title = aa.Album, year = aa.Year }).Distinct();
            //al = al.OrderByDescending(a => a.year);

            //Artists = Artists.OrderBy(a => a.Name);
            //ArtistHeader = "Artists (" + Artists.Count + ")";
            //AlbumHeader = "Albums (" + Artists.Sum(a => a.AlbumCount) + ")";
            //TrackHeader = "Tracks (" + Artists.Sum(a => a.TrackCount) + ")";
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
