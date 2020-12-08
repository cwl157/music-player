using MusicPlayer.Model;
using MusicPlayer.Services;
using MusicPlayer.ViewModels;
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System.Windows;

namespace MusicPlayer.Tests
{
    public class PlayerViewModelTests
    {
        public PlayerViewModelTests()
        {
            var app = new Application();
        }
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void AddToQueueMethod_Test()
        {
            IMusicPlayer mp = new MusicPlayerStub();
            ILibraryLoader ql = new LibraryLoaderStub();
            PlayerViewModel vm = new PlayerViewModel(mp);

            List<Song> songs = new List<Song>();
            ql.Load(null, songs);
            vm.AddToQueue(songs);
            Assert.AreEqual(2, vm.SongList.Count);
            Assert.AreEqual("2 songs - 00:30", vm.QueueInfo);
        }

        [Test]
        public void ClearQueueCommand_Test()
        {

            IMusicPlayer mp = new MusicPlayerStub();
            ILibraryLoader ql = new LibraryLoaderStub();
            PlayerViewModel vm = new PlayerViewModel(mp);

            List<Song> songs = new List<Song>();
            ql.Load(null, songs);
            vm.AddToQueue(songs);
            vm.ClearQueueCommand.Execute(null);
            Assert.AreEqual(0, vm.SongList.Count);
            Assert.AreEqual("", vm.QueueInfo);
        }

        [Test]
        public void PlayCommand_Test()
        {

            IMusicPlayer mp = new MusicPlayerStub();
            ILibraryLoader ql = new LibraryLoaderStub();
            PlayerViewModel vm = new PlayerViewModel(mp);

            List<Song> songs = new List<Song>();
            ql.Load(null, songs);
            vm.AddToQueue(songs);

            vm.SelectedIndex = 0;
            vm.SelectedSong = vm.SongList[vm.SelectedIndex];
            vm.PlayingSong = vm.SongList[1];
            vm.PlaySong.Execute(null);

            // Assert playing song properties match selected song properties
            Assert.AreEqual("Test", vm.PlayingSong.Artist);
            Assert.AreEqual("Title Two", vm.PlayingSong.Title);
            Assert.AreEqual("Test Album", vm.PlayingSong.Album);
            // Assert viewmodel state
            Assert.AreEqual("Test - Test Album [2019]", vm.ArtistAlbumInfo);
            Assert.AreEqual("2. Title Two", vm.TrackTitleInfo);
        }

        //[Test]
        //public void PauseCommand_Test()
        //{

        //    IMusicPlayer mp = new MusicPlayerStub();
        //    IQueueLoader ql = new QueueLoaderStub();
        //    SongCollection col = new SongCollection(ql);
        //    PlayerViewModel vm = new PlayerViewModel(mp, col);

        //    vm.AddToQueueCommand.Execute(null);

        //    vm.SelectedIndex = 0;
        //    vm.SelectedSong = vm.SongList[vm.SelectedIndex];
        //    vm.PlayingSong = vm.SongList[1];
        //    vm.PlaySong.Execute(null);

        //    vm.PauseSong.Execute(null);
        //}

        //[Test]
        //public void StopCommand_Test()
        //{

        //    IMusicPlayer mp = new MusicPlayerStub();
        //    IQueueLoader ql = new QueueLoaderStub();
        //    SongCollection col = new SongCollection(ql);
        //    PlayerViewModel vm = new PlayerViewModel(mp, col);

        //    vm.AddToQueueCommand.Execute(null);

        //    vm.SelectedIndex = 0;
        //    vm.SelectedSong = vm.SongList[vm.SelectedIndex];
        //    vm.PlayingSong = vm.SongList[1];
        //    vm.PlaySong.Execute(null);

        //    vm.StopSong.Execute(null);

        //    // Assert viewmodel state
        //    Assert.AreEqual(0, vm.PlayingProgress);
        //    Assert.AreEqual("00:00 / 00:10", vm.ElapsedTime);
        //}

        //[Test]
        //public void FastForwardCommand_Test()
        //{

        //    IMusicPlayer mp = new MusicPlayerStub();
        //    IQueueLoader ql = new QueueLoaderStub();
        //    SongCollection col = new SongCollection(ql);
        //    PlayerViewModel vm = new PlayerViewModel(mp, col);

        //    vm.AddToQueueCommand.Execute(null);

        //    vm.SelectedIndex = 1;
        //    vm.SelectedSong = vm.SongList[vm.SelectedIndex];
        //    vm.PlayingSong = vm.SongList[0];
        //    vm.PlaySong.Execute(null);

        //    vm.FastForwardCommand.Execute(null);

        //    // Assert viewmodel state
        //    Assert.AreEqual(10000, vm.PlayingProgress);
        //    Assert.AreEqual("00:10 / 00:20", vm.ElapsedTime);
        //}

        //[Test]
        //public void RewindCommand_Test()
        //{

        //    IMusicPlayer mp = new MusicPlayerStub();
        //    IQueueLoader ql = new QueueLoaderStub();
        //    SongCollection col = new SongCollection(ql);
        //    PlayerViewModel vm = new PlayerViewModel(mp, col);

        //    vm.AddToQueueCommand.Execute(null);

        //    vm.SelectedIndex = 1;
        //    vm.SelectedSong = vm.SongList[vm.SelectedIndex];
        //    vm.PlayingSong = vm.SongList[0];
        //    vm.PlaySong.Execute(null);

        //    //vm.StopSong.Execute(null);
        //    vm.FastForwardCommand.Execute(null);

        //    vm.RewindCommand.Execute(null);

        //    // Assert viewmodel state
        //    Assert.AreEqual(0, vm.PlayingProgress);
        //    Assert.AreEqual("00:00 / 00:20", vm.ElapsedTime);
        //}
    }
}