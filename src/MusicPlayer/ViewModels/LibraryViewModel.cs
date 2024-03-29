﻿using MusicPlayer.Infrastructure;
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
        private List<Album> _allAlbums;
        private List<Album> _filteredAlbums;
        private int _itemsPerPage;
        private int _currentPage;
        private int _totalPages;
        private IEnumerable<Song> _songs;

        public ICommand AddToQueueClick { get; private set; }
        public ICommand ClearQueueClick { get; private set; }
        public ICommand FirstPageClick { get; private set; }
        public ICommand PreviousPageClick { get; private set; }
        public ICommand NextPageClick { get; private set; }
        public ICommand LastPageClick { get; private set; }
        public event Action AddToQueueRequested = delegate { };
        public event Action ClearQueueRequested = delegate { };


        #region ViewBindedProperties
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private ObservableCollection<Album> _albums;
        public ObservableCollection<Album> Albums
        {
            get
            {
                return _albums;

            }
            set { SetProperty(ref _albums, value); }
        }

        private int _selectedIndex;

        public int SelectedIndex { get { return _selectedIndex; } set { SetProperty(ref _selectedIndex, value); } }

        private Album _selectedAlbum;
        public Album SelectedAlbum
        {
            get { return _selectedAlbum; }
            set
            {
                SetProperty(ref _selectedAlbum, value);
                if (SelectedAlbum != null)
                {
                    if (SelectedAlbum.ArtistNames.Count == 1)
                    {
                        SelectedAlbumSongList = new ObservableCollection<Song>(_songs.Where(s => s.Album == SelectedAlbum.Title && s.Year == SelectedAlbum.Year.ToString() && s.Artist == SelectedAlbum.DisplayArtist));
                    }
                    else
                    {
                        SelectedAlbumSongList = new ObservableCollection<Song>(_songs.Where(s => s.Album == SelectedAlbum.Title && s.Year == SelectedAlbum.Year.ToString()));
                    }
                }
            }
        }

        private Song _selectedAlbumSelectedSong;

        public Song SelectedAlbumSelectedSong
        {
            get { return _selectedAlbumSelectedSong; }
            set { SetProperty(ref _selectedAlbumSelectedSong, value); }
        }

        private ObservableCollection<Song> _selectedAlbumSongList;

        public ObservableCollection<Song> SelectedAlbumSongList
        {
            get { return _selectedAlbumSongList; }
            set { SetProperty(ref _selectedAlbumSongList, value); }
        }

        private string _pageText;
        public string PageText
        {
            get { return _pageText; }
            set { SetProperty(ref _pageText, value); }
        }

        private string _artistSearchText;
        
        public string ArtistSearchText
        {
            get { return _artistSearchText; }
            set
            {
                SetProperty(ref _artistSearchText, value);
                filter();
            }
        }

        private List<string> _albumYears;
        public List<String> AlbumYears
        {
            get { return _albumYears; }
            set { SetProperty(ref _albumYears, value); }
        }

        private string _selectedYear;
        public string SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                SetProperty(ref _selectedYear, value);
                filter();
            }
        }

        private bool _isViewRecent;
        public bool IsViewRecent
        {
            get { return _isViewRecent; }
            set
            {
                SetProperty(ref _isViewRecent, value);
                List<Album> r = new List<Album>();
                if (IsViewRecent)
                {
                    r = _allAlbums.Where(a => a.DateAdded >= DateTime.UtcNow.AddMonths(-3)).ToList();
                    r = r.OrderByDescending(a => a.DateAdded).ToList<Album>();
                }
                else
                {
                    r = _allAlbums;
                }

                _filteredAlbums.Clear();
                foreach (Album a in r)
                {
                    _filteredAlbums.Add(a);
                }

                _currentPage = 0;
                _totalPages = 0;

                _totalPages = r.Count / _itemsPerPage;
                if (r.Count % _itemsPerPage != 0)
                {
                    _totalPages += 1;
                }

                PageAlbums();
            }
        }


        private void filter()
        {
            List<Album> r = new List<Album>();

            if (!string.IsNullOrEmpty(_selectedYear) && !string.IsNullOrEmpty(_artistSearchText))
            {
                string searchQuery = _artistSearchText.ToLower();
                foreach (Album a in _allAlbums)
                {
                    if (a.Title.ToLower().Contains(searchQuery) && a.Year == int.Parse(_selectedYear) && !r.Contains(a))
                    {
                        r.Add(a);
                    }
                    else
                    {
                        foreach (string aa in a.ArtistNames)
                        {
                            if (aa.ToLower().Contains(searchQuery) && a.Year == int.Parse(_selectedYear) && !r.Contains(a))
                            {
                                r.Add(a);
                            }
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(_selectedYear) && string.IsNullOrEmpty(_artistSearchText))
            {
                r = _allAlbums.Where(a => a.Year == int.Parse(_selectedYear)).ToList();
            }
            else if (string.IsNullOrEmpty(_selectedYear) && !string.IsNullOrEmpty(_artistSearchText))
            {
                string searchQuery = _artistSearchText.ToLower();
                foreach (Album a in _allAlbums)
                {
                    if (a.Title.ToLower().Contains(searchQuery) && !r.Contains(a))
                    {
                        r.Add(a);
                    }
                    else
                    {
                        foreach (string aa in a.ArtistNames)
                        {
                            if (aa.ToLower().Contains(searchQuery) && !r.Contains(a))
                            {
                                r.Add(a);
                            }
                        }
                    }
                }
            }
            else if (string.IsNullOrEmpty(_selectedYear) && string.IsNullOrEmpty(_artistSearchText))
            {
                r = _allAlbums;
            }

            _filteredAlbums.Clear();
            foreach (Album a in r)
            {
                _filteredAlbums.Add(a);
            }

            _currentPage = 0;
            _totalPages = 0;

            _totalPages = r.Count / _itemsPerPage;
            if (r.Count % _itemsPerPage != 0)
            {
                _totalPages += 1;
            }

            PageAlbums();
        }

        #endregion

        public LibraryViewModel(IEnumerable<Song> songs)
        {
           
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
            _allAlbums = new List<Album>();
            _selectedAlbum = new Album();
            _albumYears = new List<string>();

            IsLoading = true;
            Refresh(songs);   
        }

        public void Refresh(IEnumerable<Song> songs)
        {
            IsLoading = true;
            Task.Run(() =>
            {
                _songs = songs;
                Load(songs);
                var years = _allAlbums.Select(a => a.Year).Distinct().OrderByDescending(y => y);
                _albumYears.Clear();
                _albumYears.Add("");
                _selectedYear = "";
                foreach (int y in years)
                {
                    _albumYears.Add(y.ToString());
                }

                _filteredAlbums = new List<Album>(_allAlbums);
                _totalPages = _allAlbums.Count / _itemsPerPage;
                if (_allAlbums.Count % _itemsPerPage != 0)
                {
                    _totalPages += 1;
                }
                // LoadingStatus = "";
                IsLoading = false;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PageAlbums();
                });
            });
        }

        private void Load(IEnumerable<Song> songs)
        {
            _allAlbums.Clear();

            // Distinct should be only by album, year and artist with comment added
            var albumNames = songs.Select(s => new { Title = s.Album, Year = s.Year, Artist = s.Artist, Id = s.Id }).Distinct();

            albumNames = albumNames.OrderByDescending(a => a.Year);
            foreach (var name in albumNames)
            {
                var albumAdded = _allAlbums.FirstOrDefault(a => a.Id == name.Id);
                if (albumAdded != null)
                {
                    continue;
                }
                //if (name.Comment.ToLower().Contains("various"))
                // {
                //     var albumAdded = _allAlbums.FirstOrDefault(a => a.Title == name.Title && a.Year.ToString() == name.Year);
                //     if (albumAdded != null)
                //     {
                //        continue;
                //    }
                // }

                //var ss = songs.Where(t => t.Album == name.Title && t.Year == name.Year && t.Artist == name.Artist);
                var ss = songs.Where(t => t.Id == name.Id);
                //if (name.Comment.ToLower().Contains("various"))
               // {
               //     ss = songs.Where(t => t.Album == name.Title && t.Year == name.Year);
               // }

                
                var newAlbum = new Album();
                newAlbum.Title = name.Title;
                newAlbum.TotalTracks = ss.Count();
                var firstSong = ss.FirstOrDefault();
                if (firstSong != null)
                {
                    //try
                    //{
                    //    var tfile = TagLib.File.Create(firstSong.FilePath);
                    //    if (tfile.Tag.Pictures.Length > 0)
                    //    {
                    //        newAlbum.AlbumArt = LoadImage(tfile.Tag.Pictures[0].Data.Data);
                    //    }
                    //}
                    //catch (Exception e)
                    //{
                    //    // TODO: error handling and logging
                    //}
                    newAlbum.Id = firstSong.Id;
                    newAlbum.DateAdded = firstSong.DateAdded;
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

        #region CommandActions
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

        /*
         *  if (SelectedAlbum.ArtistNames.Count == 1)
                    {
                        SelectedAlbumSongList = new ObservableCollection<Song>(_songs.Where(s => s.Album == SelectedAlbum.Title && s.Year == SelectedAlbum.Year.ToString() && s.Artist == SelectedAlbum.DisplayArtist));
                    }
                    else
                    {
                        SelectedAlbumSongList = new ObservableCollection<Song>(_songs.Where(s => s.Album == SelectedAlbum.Title && s.Year == SelectedAlbum.Year.ToString()));
                    }
         */

        #endregion
        private void PageAlbums()
        {
            Albums.Clear();

            int startingIndex = _currentPage * _itemsPerPage;
            int endingIndex = (_currentPage + 1) * _itemsPerPage;

            for (int i = startingIndex; i < endingIndex; i++)
            {
                if (i >= _filteredAlbums.Count)
                {
                    break;
                }
                Song firstSong = null;
                
                if (_filteredAlbums[i].ArtistNames.Count == 1)
                {
                    firstSong = _songs.FirstOrDefault(s => s.Album == _filteredAlbums[i].Title && s.Year == _filteredAlbums[i].Year.ToString() && s.Artist == _filteredAlbums[i].DisplayArtist);
                }
                else
                {
                    firstSong = _songs.FirstOrDefault(s => s.Album == _filteredAlbums[i].Title && s.Year == _filteredAlbums[i].Year.ToString());
                }

                if (_filteredAlbums[i].AlbumArt == null)
                {
                    if (firstSong != null)
                    {
                        try
                        {
                            var tfile = TagLib.File.Create(firstSong.FilePath);
                            if (tfile.Tag.Pictures.Length > 0)
                            {
                                _filteredAlbums[i].AlbumArt = LoadImage(tfile.Tag.Pictures[0].Data.Data);
                            }
                        }
                        catch (Exception e)
                        {
                            // TODO: error handling and logging
                        }
                    }
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
