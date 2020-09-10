using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // private Song _currentlyPlaying;
        private Timer _incrementPlayingProgress;
        private Timer _findSongEnd;
        private List<Song> _currentQueue;
        private Song _currentSong;

        // List<Song> Playlist;
        public MainWindow()
        {
            _currentQueue = new List<Song>();
            InitializeComponent();
            //CurrentlyPlaying = null;

            PlayingProgress.MouseDown += PlayingProgress_MouseDown;
            _incrementPlayingProgress = new Timer();
            _findSongEnd = new Timer();
            InitializeMyTimer();

            Player.LoadedBehavior = MediaState.Manual;
        }

        private void AddToQueue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(QueuePath.Text);
                List<Song> songs = new List<Song>();
                foreach (FileInfo f in dir.GetFiles())
                {
                    if (f.Extension.ToLower() == ".mp3")
                    {
                        var tfile = TagLib.File.Create(f.FullName);
                        songs.Add(new Song()
                        {
                            Artist = tfile.Tag.FirstPerformer,
                            Album = tfile.Tag.Album,
                            Title = tfile.Tag.Title,
                            TrackNumber = (int)tfile.Tag.Track,
                            Duration = tfile.Properties.Duration,
                            FilePath = f.FullName,
                            Lyrics = tfile.Tag.Lyrics,
                            AlbumArt = tfile.Tag.Pictures[0]
                        });
                    }
                }

                songs = songs.OrderBy(s => s.TrackNumber).ToList();
                
                foreach (Song s in songs)
                {
                    var i = new ListViewItem();
                    i.Content = s.TrackNumber + " - " + s.Title + " - " + s.DisplayDuration;
                    i.DataContext = s;
                    i.MouseDoubleClick += (sender, e) =>
                    {
                        StartSong();
                        StartTimers();
                    };
                    QueueView.Items.Add(i);
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
                    QueueInfoLabel.Content = _currentQueue.Count + " songs - " + totalDuration.ToString(format);
                   // ArtistAlbumLabel.Content = songs[0].Artist + " - " + songs[0].Album + " - " + totalDuration.ToString(format);
                }
            }
            catch (Exception ex)
            {
                // TODO: Empty for now so it doesn't crash
            }
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
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void I_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        // Call this method from the constructor of the form.
        private void InitializeMyTimer()
        {
            _incrementPlayingProgress.Interval = 1000;
            _incrementPlayingProgress.Elapsed += (sender, e) =>
            {
                // Increment the value of the ProgressBar a value of one each time.
                this.Dispatcher.Invoke(() =>
                {
                    PlayingProgress.Value += 1000;
                    //int selectedIndex = QueueView.SelectedIndex;
                    //var o = (ListViewItem)QueueView.Items[selectedIndex];
                    //Song s = (Song)o.DataContext;
                    // EllapsedLabel.Content = PlayingProgress.Value / 1000 + ":" + s.DisplayDuration;
                    string displayProgress = TimeSpan.FromMilliseconds(PlayingProgress.Value).ToString("mm\\:ss");
                    EllapsedLabel.Content = displayProgress + " / " + _currentSong.DisplayDuration;
                });
            };
            _findSongEnd.Interval = 1;
            _findSongEnd.Elapsed += (sender, e) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    bool done = Player.Position >= Player.NaturalDuration;
                    if (done)
                    {
                        StopTimers();
                        QueueView.SelectedItem = QueueView.Items[QueueView.SelectedIndex + 1];
                        StartSong();
                        StartTimers();
                    }
                });
            };
        }

        private void StartTimers()
        {
            _incrementPlayingProgress.Start();
            _findSongEnd.Start();
        }

        private void StopTimers()
        {
            _incrementPlayingProgress.Stop();
            _findSongEnd.Stop();
        }

        private void StartSong()
        {
            int selectedIndex = QueueView.SelectedIndex;
            var o = (ListViewItem)QueueView.Items[selectedIndex];
            Song s = (Song)o.DataContext;
            _currentSong = s;
            Player.Source = new Uri(s.FilePath);
            AlbumArtImg.Source = LoadImage(s.AlbumArt.Data.Data);
            LyricsTxtBlock.Text = s.Lyrics;
            Player.Play();
            PlayingProgress.Minimum = 0;
            PlayingProgress.Maximum = s.Duration.TotalMilliseconds;
            PlayingProgress.Value = 0;
            NowPlayingLabel.Content = s.TrackNumber + " " + s.Title;
            ArtistAlbumLabel.Content = s.Artist + " - " + s.Album;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Player.Play();
            StartTimers();
        }

        private void PlayingProgress_MouseDown(object sender, MouseButtonEventArgs e)
        {
            double p = e.GetPosition(PlayingProgress).X;
            double w = PlayingProgress.Width;
            double x =  p / w;
            PlayingProgress.Value = PlayingProgress.Maximum * x;
            Player.Position = TimeSpan.FromMilliseconds(PlayingProgress.Value);
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            Player.Pause();
            StopTimers();
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            Player.Stop();
            StopTimers();
            PlayingProgress.Value = 0;
            EllapsedLabel.Content = "00:00 / " + _currentSong.DisplayDuration;
          //  NowPlayingLabel.Content = "";
        }

        private void ClearQueueBtn_Click(object sender, RoutedEventArgs e)
        {
           // Player.Stop();
           // StopTimers();
           // PlayingProgress.Value = 0;
            QueueView.Items.Clear();
            QueuePath.Text = "";
            _currentQueue.Clear();
            QueueInfoLabel.Content = "";
           // ArtistAlbumLabel.Content = "";
        }
    }
}
