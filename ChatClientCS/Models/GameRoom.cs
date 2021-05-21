using ChatClientCS.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClientCS.Models
{
    public class GameRoom : ViewModelBase
    {
        public int GameRoomNumber { get; set; }

        public string GameName { get; set; }

        public ObservableCollection<ChatMessage> Chatter { get; set; }

        private bool _CanJoin = true;
        public bool CanJoin
        {
            get { return _CanJoin; }
            set { _CanJoin = value; OnPropertyChanged(); }
        }

        public GameRoom()
        {
            Chatter = new ObservableCollection<ChatMessage>();
        }

    }
}
