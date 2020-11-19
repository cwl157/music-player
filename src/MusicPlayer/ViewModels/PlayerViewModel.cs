using System;
using System.Collections.Generic;
using System.Text;

namespace MusicPlayer.ViewModels
{
    public class PlayerViewModel : BindableBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public PlayerViewModel()
        {
            _name = "PlayerViewModel";
        }
    }
}
