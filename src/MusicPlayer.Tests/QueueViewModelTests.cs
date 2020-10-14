using MusicPlayer.Model;
using MusicPlayer.Services;
using MusicPlayer.ViewModels;
using NUnit.Framework;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System.Windows;

namespace MusicPlayer.Tests
{
    public class QueueViewModelTests
    {
        public QueueViewModelTests()
        {
            var app = new Application();
        }
        [SetUp]
        public void Setup()
        {
           
        }

        [Test]
        public void AddToQueueCommand_Test()
        {
            IMusicPlayer mp = new MusicPlayerStub();
            IQueueLoader ql = new QueueLoaderStub();
            QueueViewModel vm = new QueueViewModel(mp, ql);

            vm.AddToQueueCommand.Execute(null);
            Assert.AreEqual(2, vm.CurrentQueue.Count);
            Assert.AreEqual("2 songs - 00:30", vm.QueueInfo);
        }

        [Test]
        public void ClearQueueCommand_Test()
        {
            
            IMusicPlayer mp = new MusicPlayerStub();
            IQueueLoader ql = new QueueLoaderStub();
            QueueViewModel vm = new QueueViewModel(mp, ql);

            vm.AddToQueueCommand.Execute(null);
            vm.ClearQueueCommand.Execute(null);
            Assert.AreEqual(0, vm.CurrentQueue.Count);
            Assert.AreEqual("", vm.QueueInfo);
        }

        [Test]
        public void PlayCommand_Test()
        {

            IMusicPlayer mp = new MusicPlayerStub();
            IQueueLoader ql = new QueueLoaderStub();
            QueueViewModel vm = new QueueViewModel(mp, ql);

            vm.AddToQueueCommand.Execute(null);

            vm.SelectedIndex = 0;
            vm.SelectedSong = vm.CurrentQueue[vm.SelectedIndex];
            vm.PlayingSong = vm.CurrentQueue[1];
            vm.PlaySong.Execute(null);

            // Assert playing song properties match selected song properties
            Assert.AreEqual("Test", vm.PlayingSong.Artist);
            Assert.AreEqual("Title", vm.PlayingSong.Title);
            Assert.AreEqual("Test Album", vm.PlayingSong.Album);
            // Assert viewmodel state
            Assert.AreEqual("Test - Test Album [2018]", vm.ArtistAlbumInfo);
            Assert.AreEqual("1. Title", vm.TrackTitleInfo);
        }

        [Test]
        public void PauseCommand_Test()
        {

            IMusicPlayer mp = new MusicPlayerStub();
            IQueueLoader ql = new QueueLoaderStub();
            QueueViewModel vm = new QueueViewModel(mp, ql);

            vm.AddToQueueCommand.Execute(null);

            vm.SelectedIndex = 0;
            vm.SelectedSong = vm.CurrentQueue[vm.SelectedIndex];
            vm.PlayingSong = vm.CurrentQueue[1];
            vm.PlaySong.Execute(null);

            vm.PauseSong.Execute(null);
        }

        [Test]
        public void StopCommand_Test()
        {

            IMusicPlayer mp = new MusicPlayerStub();
            IQueueLoader ql = new QueueLoaderStub();
            QueueViewModel vm = new QueueViewModel(mp, ql);

            vm.AddToQueueCommand.Execute(null);

            vm.SelectedIndex = 0;
            vm.SelectedSong = vm.CurrentQueue[vm.SelectedIndex];
            vm.PlayingSong = vm.CurrentQueue[1];
            vm.PlaySong.Execute(null);

            vm.StopSong.Execute(null);

            // Assert viewmodel state
            Assert.AreEqual(0, vm.PlayingProgress);
            Assert.AreEqual("00:00 / 00:10", vm.ElapsedTime);
        }

        [Test]
        public void FastForwardCommand_Test()
        {

            IMusicPlayer mp = new MusicPlayerStub();
            IQueueLoader ql = new QueueLoaderStub();
            QueueViewModel vm = new QueueViewModel(mp, ql);

            vm.AddToQueueCommand.Execute(null);

            vm.SelectedIndex = 1;
            vm.SelectedSong = vm.CurrentQueue[vm.SelectedIndex];
            vm.PlayingSong = vm.CurrentQueue[0];
            vm.PlaySong.Execute(null);

            vm.FastForwardCommand.Execute(null);

            // Assert viewmodel state
            Assert.AreEqual(10000, vm.PlayingProgress);
            Assert.AreEqual("00:10 / 00:20", vm.ElapsedTime);
        }

        [Test]
        public void RewindCommand_Test()
        {

            IMusicPlayer mp = new MusicPlayerStub();
            IQueueLoader ql = new QueueLoaderStub();
            QueueViewModel vm = new QueueViewModel(mp, ql);

            vm.AddToQueueCommand.Execute(null);

            vm.SelectedIndex = 1;
            vm.SelectedSong = vm.CurrentQueue[vm.SelectedIndex];
            vm.PlayingSong = vm.CurrentQueue[0];
            vm.PlaySong.Execute(null);

            //vm.StopSong.Execute(null);
            vm.FastForwardCommand.Execute(null);

            vm.RewindCommand.Execute(null);

            // Assert viewmodel state
            Assert.AreEqual(0, vm.PlayingProgress);
            Assert.AreEqual("00:00 / 00:20", vm.ElapsedTime);
        }
    }
}