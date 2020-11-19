using MusicPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, IMusicPlayer
    {
       // protected MediaElement Player { get; set; }
        public Window1()
        {
            InitializeComponent();
            SongPlayer.LoadedBehavior = MediaState.Manual;
            DataContext = new Window1ViewModel(this);
            
        }

        #region IMusicPlayer
        public void Play(Uri filePath)
        {
            SongPlayer.Source = filePath;
            SongPlayer.Play();
        }

        public void Play()
        {
            SongPlayer.Play();
        }

        public void Pause()
        {
            SongPlayer.Pause();
        }

        public void Stop()
        {
            SongPlayer.Stop();

        }

        public void FastForward(double milliseconds)
        {
            SongPlayer.Position += TimeSpan.FromMilliseconds(milliseconds);
        }

        public void Rewind(double milliseconds)
        {
            SongPlayer.Position -= TimeSpan.FromMilliseconds(milliseconds);
        }

        public bool IsDone()
        {
            return SongPlayer.Position >= SongPlayer.NaturalDuration;
        }
        #endregion
    }
}
