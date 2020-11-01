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
using MusicPlayer.Services;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMusicPlayer
    {
        private QueueViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();
            Player.LoadedBehavior = MediaState.Manual;
            IQueueLoader ql = new FileQueueLoader();
            SongCollection collection = new SongCollection(ql);
            _vm = new QueueViewModel(this, collection);
            DataContext = _vm;
        }

        public void ListViewItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _vm.PlaySong.Execute(null);
        }

        #region IMusicPlayer
        public void Play(Uri filePath)
        {
            Player.Source = filePath;
            Player.Play();
        }

        public void Play()
        {
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

        public void FastForward(double milliseconds)
        {
            Player.Position += TimeSpan.FromMilliseconds(milliseconds);
        }
       
        public void Rewind(double milliseconds)
        {
            Player.Position -= TimeSpan.FromMilliseconds(milliseconds);
        }

        public bool IsDone()
        {
            return Player.Position >= Player.NaturalDuration;
        }
        #endregion
    }
}
