using MusicPlayer.Infrastructure;
using MusicPlayer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MusicPlayer.ViewModels
{
    public class LibraryViewModel : BindableBase
    {
        public ICommand AddToQueueClick { get; private set; }
        public ICommand ClearQueueClick { get; private set; }
        public ICommand FirstPageClick { get; private set; }
        public ICommand PreviousPageClick { get; private set; }
        public ICommand NextPageClick { get; private set; }
        public ICommand LastPageClick { get; private set; }

        public event Action AddToQueueRequested = delegate { };
        public event Action ClearQueueRequested = delegate { };

        private List<Album> _allAlbums;
        private List<Artist> _allArtists;
        private List<Album> _filteredAlbums;
        private int _itemsPerPage;
        private int _currentPage;
        private int _totalPages;

        private ObservableCollection<Artist> _artists;
        public ObservableCollection<Artist> Artists { get { return _artists; } set { SetProperty(ref _artists, value); } }

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
                if (_selectedArtist != null)
                {
                    _albums.Clear();
                    IEnumerable<Album> r = Enumerable.Empty<Album>();
                    if (_selectedArtist.Name.Contains("Show All ("))
                    //if (_selectedIndex == 0)
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
        }

        private int _selectedIndex;

        public int SelectedIndex { get { return _selectedIndex; } set { SetProperty(ref _selectedIndex, value); } }

        private Album _selectedAlbum;
        public Album SelectedAlbum { get { return _selectedAlbum; } set { SetProperty(ref _selectedAlbum, value); } }

        private string _pageText;
        public string PageText
        {
            get { return _pageText; }
            set { SetProperty(ref _pageText, value); }
        }

        //private BitmapImage _ImageData;
        //public BitmapImage ImageData
        //{
        //    get { return this._ImageData; }
        //    set { SetProperty(ref _ImageData, value); }
        //}

        private string _artistSearchText;
        
        public string ArtistSearchText
        {
            get { return _artistSearchText; }
            set
            {
                SetProperty(ref _artistSearchText, value);
              //  _albums.Clear();
                //_artists.Clear();
                List<Album> r = new List<Album>();
                //if (_selectedArtist.Name == "Show All")
                if (string.IsNullOrEmpty(_artistSearchText))
                {
                    r = _allAlbums;
                }
                else
                {
                    string searchQuery = _artistSearchText.ToLower();
                    r = _allAlbums.Where(a => a.DisplayArtist.ToLower().Contains(searchQuery) || a.Title.ToLower().Contains(searchQuery)).ToList();
                }

                _filteredAlbums.Clear();
                foreach (Album a in r)
                {
                    _filteredAlbums.Add(a);
                }

                // recalc pages based on search results
               // _itemsPerPage = 10;
                _currentPage = 0;
                _totalPages = 0;
                // page it

                _totalPages = r.Count / _itemsPerPage;
                if (r.Count % _itemsPerPage != 0)
                {
                    _totalPages += 1;
                }

              //  Albums.Clear();
                PageAlbums();
                // _currentPage = 1;
                // _itemsPerPage = 10;
                //int startingIndex = _currentPage * _itemsPerPage;
                //int endingIndex = (_currentPage + 1) * _itemsPerPage;

                //for (int i = startingIndex; i < endingIndex; i++)
                //{
                //    if (i >= .Count)
                //    {
                //        break;
                //    }
                //    Albums.Add(r[i]);
                //}

                //foreach (Album aa in r)
                //{
                //    _albums.Add(aa);
                //}
                //_currentPage = 0;
            }
        }


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
        // for this code image needs to be a project resource
        //private BitmapImage LoadImage(string filename)
        //{
        //    return new BitmapImage(new Uri("pack://application:,,,/" + filename));
        //}
        public LibraryViewModel(IEnumerable<Song> songs)
        {
            //ImageData = LoadImage(@"D:\My Music\Full Albums\ANCST\Summits of Despondency\cover.jpg");
            AddToQueueClick = new CommandHandler(() => AddToQueueAction(), () => true);
            ClearQueueClick = new CommandHandler(() => ClearQueueAction(), () => true);
            FirstPageClick = new CommandHandler(() => FirstPageAction(), () => true);
            PreviousPageClick = new CommandHandler(() => PreviousPageAction(), () => true);
            NextPageClick = new CommandHandler(() => NextPageAction(), () => true);
            LastPageClick = new CommandHandler(() => LastPageAction(), () => true);
            Albums = new ObservableCollection<Album>();
            _itemsPerPage = 10;
            _currentPage = 0;
            _totalPages = 0;

            _allArtists = new List<Artist>();
            _allAlbums = new List<Album>();
//            _filteredAlbums = new List<Album>();

            _selectedArtist = new Artist();
            _selectedAlbum = new Album();

            Load(songs);
            _filteredAlbums = new List<Album>(_allAlbums);
            _totalPages = _allAlbums.Count / _itemsPerPage;
            if (_allAlbums.Count % _itemsPerPage != 0)
            {
                _totalPages += 1;
            }
            PageAlbums();
        }

        public void Load(IEnumerable<Song> songs)
        {
            _allArtists.Clear();
            _allAlbums.Clear();
            IEnumerable<string> artistNames = songs.Select(s => s.Artist).Distinct();
            artistNames = artistNames.OrderBy(s => s);
            foreach (string s in artistNames)
            {
                var newArtist = new Artist() { Name = s };
                var artistSongs = songs.Where(a => a.Artist == s);
                var artistAlbums = artistSongs.Select(aa => new { title = aa.Album, year = aa.Year }).Distinct();
                newArtist.AlbumCount = artistAlbums.Count();
                newArtist.TrackCount = artistSongs.Count();
                _allArtists.Add(newArtist);
            }

            var albumNames = songs.Select(s => new { Title = s.Album, Year = s.Year }).Distinct();
            albumNames = albumNames.OrderByDescending(a => a.Year);
            foreach (var name in albumNames)
            {
                var ss = songs.Where(t => t.Album == name.Title && t.Year == name.Year);
                var newAlbum = new Album();
                newAlbum.Title = name.Title;
                newAlbum.TotalTracks = ss.Count();
                var firstSong = ss.FirstOrDefault();
                if (firstSong != null)
                {
                    try
                    {
                        var tfile = TagLib.File.Create(firstSong.FilePath);
                        if (tfile.Tag.Pictures.Length > 0)
                        {
                            newAlbum.AlbumArt = LoadImage(tfile.Tag.Pictures[0].Data.Data);
                        }
                    }
                    catch (Exception e)
                    {
                        // TODO: error handling and logging
                    }
                }
                int outYear = 0;
                int.TryParse(ss.First().Year, out outYear);
                newAlbum.Year = outYear;
                newAlbum.ArtistNames = new List<string>();
                IEnumerable<string> artists = ss.Select(s => s.Artist).Distinct();
                foreach (string ns in artists)
                {
                    newAlbum.ArtistNames.Add(ns);
                }
                if (newAlbum.ArtistNames.Count == 0)
                {
                    newAlbum.DisplayArtist = "Unknown";
                }
                else if (newAlbum.ArtistNames.Count == 1)
                {
                    newAlbum.DisplayArtist = newAlbum.ArtistNames.First();
                }
                else if (newAlbum.ArtistNames.Count > 1)
                {
                    newAlbum.DisplayArtist = "Various Artists";
                }
                else
                {
                    newAlbum.DisplayArtist = "Unknown";
                }
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

            _allAlbums = _allAlbums.OrderBy(a => a.DisplayArtist).ToList();

          //  Albums = new ObservableCollection<Album>(_allAlbums);
            _allArtists.Insert(0, new Artist { Name = "Show All (" + _allArtists.Count + ")", AlbumCount = Albums.Count, TrackCount = Albums.Sum(a => a.TotalTracks) });
            Artists = new ObservableCollection<Artist>(_allArtists);
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.DecodePixelWidth = 160;
                image.DecodePixelHeight = 160;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();

            return image;
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

        private void FirstPageAction()
        {
           if (_currentPage != 0)
            {
                _currentPage = 0;
                PageAlbums();
            }
        }

        private void PreviousPageAction()
        {
            if (_currentPage > 0)
            {
                _currentPage--;
                PageAlbums();
            }
        }

        private void NextPageAction()
        {
            if (_currentPage < _totalPages - 1)
            {
                _currentPage++;
                PageAlbums();
            }
        }

        private void LastPageAction()
        {
            if (_currentPage != _totalPages - 1)
            {
                _currentPage = _totalPages - 1;
                PageAlbums();
            }
        }

        private void PageAlbums()
        {
            Albums.Clear();

           // _currentPage = 1;
           // _itemsPerPage = 10;
            int startingIndex = _currentPage * _itemsPerPage;
            int endingIndex = (_currentPage + 1) * _itemsPerPage;

            for (int i = startingIndex; i < endingIndex; i++)
            {
                if (i >= _filteredAlbums.Count)
                {
                    break;
                }
                Albums.Add(_filteredAlbums[i]);
            }

            int displayStartingIndex = startingIndex + 1;
            int displayEndingIndex = endingIndex;
            if (endingIndex >= _filteredAlbums.Count)
            {
                displayEndingIndex = _filteredAlbums.Count;
            }
            PageText = displayStartingIndex + " - " + displayEndingIndex + " / " + _filteredAlbums.Count; //10 - 19 / 100
        }
    }
}
