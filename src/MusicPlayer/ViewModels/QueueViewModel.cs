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

namespace MusicPlayer.ViewModels
{
    public class QueueViewModel : INotifyPropertyChanged
    {
        private Timer _incrementPlayingProgress;
        private Timer _findSongEnd;
        private int _playingIndex;
        private readonly IMusicPlayer _player;
        private readonly IQueueLoader _fileQueueLoader;
        private double _seconds;

        public QueueViewModel(IMusicPlayer m, IQueueLoader ql)
        {
            if (DesignerProperties.GetIsInDesignMode(
                new System.Windows.DependencyObject())) return;

            _seconds = 0;
            _currentQueue = new ObservableCollection<Song>();
            _playingSong = new Song();
            QueueInfo = "";
            _player = m;
            _fileQueueLoader = ql;
            ElapsedTime = "00:00 / 00:00";
            _incrementPlayingProgress = new Timer();
            _incrementPlayingProgress.Interval = 1000;
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
                        if (++_playingIndex < CurrentQueue.Count)
                        {

                            SelectedSong = CurrentQueue[_playingIndex];
                            PlaySongAction();
                        }
                        else
                        {
                            StopSongAction();
                        }
                    }
                });
            };
        }

        #region ViewBindedProperties
        public string QueueFilePath { get; set; }

        private ObservableCollection<Song> _currentQueue;
        public ObservableCollection<Song> CurrentQueue
        {
            get
            {
                if (_currentQueue == null)
                {
                    return new ObservableCollection<Song>();
                }
                else
                {
                    return _currentQueue;
                }
            }
        }
        
        private string _queueInfo;
        public string QueueInfo
        {
            get { return _queueInfo; }
            set
            {
                _queueInfo = value;
                OnPropertyChanged();
            }
        }

        private string _artistAlbumInfo;
        public string ArtistAlbumInfo {
            get { return _artistAlbumInfo; }
            set
            {
                _artistAlbumInfo = value;
                OnPropertyChanged();
            }
        }

        private string _trackTitleInfo;
        public string TrackTitleInfo
        {
            get { return _trackTitleInfo; }
            set
            {
                _trackTitleInfo = value;
                OnPropertyChanged();
            }
        }

        private Song _selectedSong;
        public Song SelectedSong
        {
            get { return _selectedSong; }
            set
            {
                _selectedSong = value;
                OnPropertyChanged();
            }
        }

        private Song _playingSong;
        public Song PlayingSong
        {
            get { return _playingSong; }
            set
            {
                _playingSong = value;
                OnPropertyChanged();
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; OnPropertyChanged(); }
        }

        private double _playingProgress;
        public double PlayingProgress
        {
            get { return _playingProgress; }
            set { _playingProgress = value; OnPropertyChanged(); }
        }

        private string _elapsedTime;
        public string ElapsedTime
        {
            get { return _elapsedTime; }
            set { _elapsedTime = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands
        private ICommand _addToQueueCommand;
        public ICommand AddToQueueCommand
        {
            get
            {
                return _addToQueueCommand ?? (_addToQueueCommand = new CommandHandler(() => AddToQueueAction(), () => true));
            }
        }
        private void AddToQueueAction()
        {
            var songs = _fileQueueLoader.Load(QueueFilePath);
            foreach (Song s in songs)
            {
                _currentQueue.Add(s);
            }

            if (_currentQueue.Count > 0)
            {
                double queueDuration = _currentQueue.Sum(d => d.Duration.TotalSeconds);
                string format = "hh\\:mm\\:ss";
                if (queueDuration < 3600)
                {
                    format = "mm\\:ss";
                }
                TimeSpan totalDuration = TimeSpan.FromSeconds(queueDuration);
                QueueInfo = _currentQueue.Count + " songs - " + totalDuration.ToString(format);
            }
        }

        private ICommand _clearQueueCommand;
        public ICommand ClearQueueCommand
        {
            get
            {
                return _clearQueueCommand ?? (_clearQueueCommand = new CommandHandler(() => ClearQueueAction(), () => true));
            }
        }
        private void ClearQueueAction()
        {
            _currentQueue.Clear();
            QueueInfo = "";
        }

        private ICommand _playCommand;
        public ICommand PlaySong
        {
            get
            {
                return _playCommand ?? (_playCommand = new CommandHandler(() => PlaySongAction(), () => true));
            }
        }
        private void PlaySongAction()
        {
            if (PlayingSong.FilePath != SelectedSong.FilePath)
            {
                StopSongAction();
                PlayingSong = SelectedSong;
                _playingIndex = SelectedIndex;
                ArtistAlbumInfo = PlayingSong.Artist + " - " + PlayingSong.Album;
                TrackTitleInfo = PlayingSong.TrackNumber + ". " + PlayingSong.Title;
            }
            StartTimers();
            _player.Play(new Uri(PlayingSong.FilePath));
        }


        private ICommand _pauseCommand;
        public ICommand PauseSong
        {
            get
            {
                return _pauseCommand ?? (_pauseCommand = new CommandHandler(() => PauseSongAction(), () => true));
            }
        }
        private void PauseSongAction()
        {
            StopTimers();
            _player.Pause();
        }

        private ICommand _stopCommand;
        public ICommand StopSong
        {
            get
            {
                return _stopCommand ?? (_stopCommand = new CommandHandler(() => StopSongAction(), () => true));
            }
        }
        private void StopSongAction()
        {
            _player.Stop();
            StopTimers();
            PlayingProgress = 0;
            _seconds = 0;
            ElapsedTime = "00:00 / " + PlayingSong.DisplayDuration;
        }

        private ICommand _fastforwardCommand;
        public ICommand FastForwardCommand
        {
            get
            {
                return _fastforwardCommand ?? (_fastforwardCommand = new CommandHandler(() => FastForwardAction(), () => true));
            }
        }
        private void FastForwardAction()
        {
            _player.FastForward(10000);

            _seconds += 10000;
            PlayingProgress += 10000;

            string displayProgress = TimeSpan.FromMilliseconds(_seconds).ToString("mm\\:ss");
            ElapsedTime = displayProgress + " / " + PlayingSong.DisplayDuration;
        }

        private ICommand _rewindCommand;
        public ICommand RewindCommand
        {
            get
            {
                return _rewindCommand ?? (_rewindCommand = new CommandHandler(() => RewindAction(), () => true));
            }
        }
        private void RewindAction()
        {
            _player.Rewind(10);
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

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region privateUtilities
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
