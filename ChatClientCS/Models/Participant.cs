using ChatClientCS.ViewModels;
using System.Collections.ObjectModel;

namespace ChatClientCS.Models
{
    public class Participant : ViewModelBase
    {
        public string Name { get; set; }
        public byte[] Photo { get; set; }
        
        private int _Score;

        public int Score
        {
            get { return _Score; }
            set { _Score = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ChatMessage> Chatter { get; set; }

        private bool _isLoggedIn = true;
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set { _isLoggedIn = value; OnPropertyChanged(); }
        }

        private bool _hasSentNewMessage;
        public bool HasSentNewMessage
        {
            get { return _hasSentNewMessage; }
            set { _hasSentNewMessage = value; OnPropertyChanged(); }
        }

        private bool _isTyping;
        public bool IsTyping
        {
            get { return _isTyping; }
            set { _isTyping = value; OnPropertyChanged(); }
        }

        public Participant() { Chatter = new ObservableCollection<ChatMessage>(); }
    }
}