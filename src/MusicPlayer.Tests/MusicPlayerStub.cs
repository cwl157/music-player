using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MusicPlayer.Tests
{
    internal class MusicPlayerStub : IMusicPlayer
    {
        private string _state;
        public string State => _state;

        public MusicPlayerStub()
        {
            _state = "unknown";
        }

        public void FastForward(double milliseconds)
        {
            Debug.WriteLine("FastForward");
            _state = "FastForward";
        }

        public bool IsDone()
        {
            Debug.WriteLine("IsDone");
            return false;
        }

        public void Pause()
        {
            _state = "Paused";
            Debug.WriteLine("Pause");
        }

        public void Play(Uri filePath)
        {
            Debug.WriteLine("Play");
        }

        public void Rewind(int seconds)
        {
            Debug.WriteLine("Rewind");
        }

        public void Stop()
        {
            Debug.WriteLine("Stop");
        }
    }
}
