using MusicPlayer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Timers;
using MusicPlayer.Infrastructure;
using System.Dynamic;
using MusicPlayer.Services;

namespace MusicPlayer.ViewModels
{
    public class PlayerViewModel : BindableBase
    {
        private Timer _incrementPlayingProgress;
        private Timer _findSongEnd;
        private int _playingIndex;
        private readonly IMusicPlayer _player;
       // private SongCollection _currentQueue;
        private double _seconds;

       // public ICommand AddToQueueCommand { get; private set; }
        public ICommand ClearQueueCommand { get; private set; }
        public ICommand PlaySong { get; private set; }
        public ICommand PauseSong { get; private set; }
        public ICommand StopSong { get; private set; }
        public ICommand FastForwardCommand { get; private set; }
        public ICommand RewindCommand { get; private set; }

        public PlayerViewModel(IMusicPlayer m)
        {
            if (DesignerProperties.GetIsInDesignMode(
                new System.Windows.DependencyObject())) return;

            _seconds = 0;

            _playingSong = new Song();
            QueueInfo = "";
            _player = m;
            ElapsedTime = "00:00 / 00:00";
            _incrementPlayingProgress = new Timer();
            _incrementPlayingProgress.Interval = 1000;
            // _currentQueue = new SongCollection(null);
            SongList = new ObservableCollection<Song>();
            _incrementPlayingProgress.Elapsed += (sender, e) =>
            {
                _seconds += 1000;
                PlayingProgress += 1000;

                string displayProgress = TimeSpan.FromMilliseconds(_seconds).ToString("mm\\:ss");
                ElapsedTime = displayProgress + " / " + PlayingSong.DisplayDuration;
            };
            _findSongEnd = new Timer();
            _findSongEnd.Interval = 1;
            _findSongEnd.Elapsed += (sender, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (_player.IsDone())
                    {
                        Debug.WriteLine("Done");
                        if (++_playingIndex < SongList.Count)
                        {

                            SelectedSong = SongList[_playingIndex];
                            PlaySongAction();
                        }
                        else
                        {
                            StopSongAction();
                        }
                    }
                });
            };

            //AddToQueueCommand = new CommandHandler(() => AddToQueueAction(), () => true);
            ClearQueueCommand = new CommandHandler(() => ClearQueueAction(), () => true);
            PlaySong = new CommandHandler(() => PlaySongAction(), () => true);
            PauseSong = new CommandHandler(() => PauseSongAction(), () => true);
            StopSong = new CommandHandler(() => StopSongAction(), () => true);
            FastForwardCommand = new CommandHandler(() => FastForwardAction(), () => true);
            RewindCommand = new CommandHandler(() => RewindAction(), () => true);
        }

        #region ViewBindedProperties
     //   public string QueueFilePath { get; set; }
        public ObservableCollection<Song> SongList { get; private set; }

        private string _queueInfo;
        public string QueueInfo
        {
            get { return _queueInfo; }
            set
            {
                SetProperty(ref _queueInfo, value);
            }
        }

        private string _artistAlbumInfo;
        public string ArtistAlbumInfo {
            get { return _artistAlbumInfo; }
            set
            {
                SetProperty(ref _artistAlbumInfo, value);
            }
        }

        private string _trackTitleInfo;
        public string TrackTitleInfo
        {
            get { return _trackTitleInfo; }
            set
            {
                SetProperty(ref _trackTitleInfo, value);
            }
        }

        private Song _selectedSong;
        public Song SelectedSong
        {
            get { return _selectedSong; }
            set
            {
                SetProperty(ref _selectedSong, value);
            }
        }

        private Song _playingSong;
        public Song PlayingSong
        {
            get { return _playingSong; }
            set
            {
                SetProperty(ref _playingSong, value);
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                SetProperty(ref _selectedIndex, value);
            }
        }

        private BitmapImage _selectedAlbumArt;
        public BitmapImage SelectedAlbumArt
        {
            get { return _selectedAlbumArt; }
            set { SetProperty(ref _selectedAlbumArt, value); }
        }

        private double _playingProgress;
        public double PlayingProgress
        {
            get { return _playingProgress; }
            set
            {
                SetProperty(ref _playingProgress, value);
            }
        }

        private string _elapsedTime;
        public string ElapsedTime
        {
            get { return _elapsedTime; }
            set
            {
                SetProperty(ref _elapsedTime, value);
            }
        }

        #endregion

        public void AddToQueue(IEnumerable<Song> songs)
        {
            songs = songs.OrderByDescending(s=>s.Year).ThenBy(s => s.Album).ThenBy(s => s.TrackNumber);
            foreach (Song s in songs)
            {
                SongList.Add(s);
            }

            if (SongList.Count > 0)
            {
                double queueDuration = SongList.Sum(s => s.Duration.TotalSeconds);
                string format = "hh\\:mm\\:ss";
                if (queueDuration < 3600)
                {
                    format = "mm\\:ss";
                }
                TimeSpan totalDuration = TimeSpan.FromSeconds(queueDuration);
                QueueInfo = SongList.Count + " songs - " + totalDuration.ToString(format);
            }
            SelectedIndex = 0;
        }

        #region CommandActions
        //private void AddToQueueAction()
        //{
        //    _currentQueue.Load(QueueFilePath);

        //    if (_currentQueue.SongList.Count > 0)
        //    {
        //        double queueDuration = _currentQueue.TotalSeconds();
        //        string format = "hh\\:mm\\:ss";
        //        if (queueDuration < 3600)
        //        {
        //            format = "mm\\:ss";
        //        }
        //        TimeSpan totalDuration = TimeSpan.FromSeconds(queueDuration);
        //        QueueInfo = _currentQueue.SongList.Count + " songs - " + totalDuration.ToString(format);
        //    }
        //}

        private void ClearQueueAction()
        {
            SongList.Clear();
            QueueInfo = "";
        }

        private void PlaySongAction()
        {
            if (PlayingSong.FilePath != SelectedSong.FilePath)
            {
                StopSongAction();
                PlayingSong = SelectedSong;
                var tfile = TagLib.File.Create(PlayingSong.FilePath);
                if (tfile.Tag.Pictures.Length > 0)
                {
                    SelectedAlbumArt = LoadImage(tfile.Tag.Pictures[0].Data.Data);
                }
                else
                {
                    SelectedAlbumArt = null;
                }
                _playingIndex = SelectedIndex;
                ArtistAlbumInfo = PlayingSong.Artist + " - " + PlayingSong.Album + " [" + PlayingSong.Year + "]";
                TrackTitleInfo = PlayingSong.TrackNumber + ". " + PlayingSong.Title;
                _player.Play(new Uri(PlayingSong.FilePath));
            }
            else
            {
                _player.Play();
            }
            StartTimers();
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
                image.DecodePixelHeight = 256;
                image.DecodePixelWidth = 256;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void PauseSongAction()
        {
            StopTimers();
            _player.Pause();
        }

        private void StopSongAction()
        {
            _player.Stop();
            StopTimers();
            PlayingProgress = 0;
            _seconds = 0;
            ElapsedTime = "00:00 / " + PlayingSong.DisplayDuration;
        }

        private void FastForwardAction()
        {
            _player.FastForward(10000);

            _seconds += 10000;
            PlayingProgress += 10000;

            string displayProgress = TimeSpan.FromMilliseconds(_seconds).ToString("mm\\:ss");
            ElapsedTime = displayProgress + " / " + PlayingSong.DisplayDuration;
        }

        private void RewindAction()
        {
            _player.Rewind(10000);
            _seconds -= 10000;
            PlayingProgress -= 10000;
            if (_seconds < 0)
            {
                _seconds = 0;
            }

            if (PlayingProgress < 0)
            {
                PlayingProgress = 0;
            }

            string displayProgress = TimeSpan.FromMilliseconds(_seconds).ToString("mm\\:ss");
            ElapsedTime = displayProgress + " / " + PlayingSong.DisplayDuration;
        }

        #endregion

        #region TimerControls
        private void StopTimers()
        {
            _incrementPlayingProgress.Stop();
            _findSongEnd.Stop();
        }

        private void StartTimers()
        {
            _incrementPlayingProgress.Start();
            _findSongEnd.Start();
        }
        #endregion
    }
}
