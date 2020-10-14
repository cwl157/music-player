using System;
using System.Collections.Generic;
using System.Text;

namespace MusicPlayer
{
    public interface IMusicPlayer
    {
        void Play(Uri filePath);
        void Play();
        void Pause();
        void Stop();
        void FastForward(double milliseconds);
        void Rewind(double milliseconds);
        bool IsDone();
    }
}
