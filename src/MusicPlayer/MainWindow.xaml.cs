using MusicPlayer.Model;
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
using MusicPlayer.ViewModels;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMusicPlayer
    {
        public MainWindow()
        {
            InitializeComponent();
            Player.LoadedBehavior = MediaState.Manual;
            IQueueLoader ql = new FileQueueLoader();
            var vm = new QueueViewModel(this, ql);
            DataContext = vm;
        }

        #region IMusicPlayer
        public string State => "";
        public void Play(Uri filePath)
        {

            if (Player.Source == null || filePath.AbsoluteUri != Player.Source.AbsoluteUri)
            {
                Player.Source = filePath;
            }
            
            Player.Play();
        }

        public void Pause()
        {
            Player.Pause();
        }

        public void Stop()
        {
            Player.Stop();
            
        }

        public void FastForward(int seconds)
        {
            Player.Position += TimeSpan.FromSeconds(seconds);
        }
       
        public void Rewind(int seconds)
        {
            Player.Position -= TimeSpan.FromSeconds(seconds);
        }

        public bool IsDone()
        {
            return Player.Position >= Player.NaturalDuration;
        }
        #endregion
    }
}
