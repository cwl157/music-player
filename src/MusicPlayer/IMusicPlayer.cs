﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MusicPlayer
{
    public interface IMusicPlayer
    {
        string State { get; }
        void Play(Uri filePath);
        void Pause();
        void Stop();
        void FastForward(int seconds);
        void Rewind(int seconds);
        bool IsDone();
    }
}
