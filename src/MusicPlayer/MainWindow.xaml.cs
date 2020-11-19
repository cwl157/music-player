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
    public partial class MainWindow : UserControl
    {
        //private MainWindowViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ListViewItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
        //    _vm.PlaySong.Execute(null);
        }
    }
}
