using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using System.Drawing;
using ChatClientCS.Services;
using ChatClientCS.Enums;
using ChatClientCS.Models;
using ChatClientCS.Commands;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Ink;

namespace ChatClientCS.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IChatService chatService;
        private IDialogService dialogService;
        private TaskFactory ctxTaskFactory;
        private const int MAX_IMAGE_WIDTH = 150;
        private const int MAX_IMAGE_HEIGHT = 150;

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        private string _CurrentAnswer;
        public string CurrentAnswer
        {
            get { return _CurrentAnswer; }
            set
            {
                _CurrentAnswer = value;
                OnPropertyChanged();
            }
        }


        private string _Right = "오답";
        public string Right
        {
            get { return _Right; }
            set
            {
                _Right = value;
                OnPropertyChanged();
            }
        }

        private string _GameName ;
        public string GameName
        {
            get { return _GameName; }
            set
            {
                _GameName = value;
                OnPropertyChanged();
            }
        }

        private string _NotiMsg;
        public string NotiMsg
        {
            get { return _NotiMsg; }
            set
            {
                _NotiMsg = value;
                OnPropertyChanged();
            }
        }



        private string _profilePic;
        public string ProfilePic
        {
            get { return _profilePic; }
            set
            {
                _profilePic = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Participant> _participants = new ObservableCollection<Participant>();
        public ObservableCollection<Participant> Participants
        {
            get { return _participants; }
            set
            {
                _participants = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ChatMessage> _ChatLog = new ObservableCollection<ChatMessage>();
        public ObservableCollection<ChatMessage> ChatLog
        {
            get { return _ChatLog; }
            set
            {
                _ChatLog = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<GameRoom> _GameList = new ObservableCollection<GameRoom>();
        public ObservableCollection<GameRoom> GameList
        {
            get { return _GameList; }
            set
            {
                _GameList = value;
                OnPropertyChanged();
            }
        }

        private StrokeCollection _Strokes = new StrokeCollection();

        public StrokeCollection Strokes
        {
            get { return _Strokes; }
            set
            {
                _Strokes = value;
                OnPropertyChanged();
            }
        }


        private Participant _selectedParticipant;
        public Participant SelectedParticipant
        {
            get { return _selectedParticipant; }
            set
            {
                _selectedParticipant = value;
                if (SelectedParticipant.HasSentNewMessage) SelectedParticipant.HasSentNewMessage = false;
                OnPropertyChanged();
            }
        }

        private Participant _SelectedGameRoom;
        public Participant SelectedGameRoom
        {
            get { return _SelectedGameRoom; }
            set
            {
                _SelectedGameRoom = value;
                //if (SelectedParticipant.HasSentNewMessage) SelectedParticipant.HasSentNewMessage = false;
                OnPropertyChanged();
            }
        }

        private UserModes _userMode;
        public UserModes UserMode
        {
            get { return _userMode; }
            set
            {
                _userMode = value;
                OnPropertyChanged();
            }
        }

        private string _textMessage;
        public string TextMessage
        {
            get { return _textMessage; }
            set
            {
                _textMessage = value;
                OnPropertyChanged();
            }
        }

        private string _MainSound;

        public string MainSound
        {
            get { return _MainSound; }
            set
            {
                _MainSound = value;
                OnPropertyChanged();
            }
        }

        private string _Effect;

        public string Effect
        {
            get { return _Effect; }
            set
            {
                _Effect = value;
                OnPropertyChanged();
            }
        }


        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        private bool _IsHost;
        public bool IsHost
        {
            get { return _IsHost; }
            set
            {
                _IsHost = value;
                OnPropertyChanged();
            }
        }

        private bool _IsInGame;
        public bool IsInGame
        {
            get { return _IsInGame; }
            set
            {
                _IsInGame = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged();
            }
        }

        #region Begin Game Command

        private ICommand _BeginGameCommand;

        public ICommand BeginGameCommand
        {
            get
            {
                return _BeginGameCommand ?? (_BeginGameCommand =
                    new RelayCommandAsync(() => BeginGame(), (o) => CanBeginGame(), (ex) => NotiMsg = ex.Message));
            }
        }

        private bool CanBeginGame()
        {
            if (IsLoggedIn && !IsInGame && Participants.Count >= 2) return true;
            else return false;
        }
        private async Task<bool> BeginGame()
        {
            try
            {
                await chatService.BeginGameAsync();
                return true;
            }
            catch (Exception) { return false; }
        }
        #endregion

        #region End Game Command

        private ICommand _EndGameCommand;

        public ICommand EndGameCommand
        {
            get
            {
                return _EndGameCommand ?? (_EndGameCommand =
                    new RelayCommandAsync(() => EndGame(), (o) => CanEndGame(), (ex) => NotiMsg = ex.Message));
            }
        }

        private bool CanEndGame()
        {
            if (IsLoggedIn && IsInGame) return true;
            else return false;
        }
        private async Task<bool> EndGame()
        {
            try
            {
                await chatService.EndGameAsync();
                return true;
            }
            catch (Exception) { return false; }
        }


        #endregion

        #region Connect Command
        private ICommand _connectCommand;
        public ICommand ConnectCommand
        {
            get
            {
                return _connectCommand ?? (_connectCommand = new RelayCommandAsync(() => Connect()));
            }
        }

        private async Task<bool> Connect()
        {
            try
            {
                await chatService.ConnectAsync();
                IsConnected = true;
                return true;
            }
            catch (Exception) { return false; }
        }
        #endregion

        #region Login Command
        private ICommand _loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand =
                    new RelayCommandAsync(() => Login(), (o) => CanLogin(), (ex) => NotiMsg = ex.Message));
            }
        }

        private async Task<bool> Login()
        {
            try
            {
                List<User> users = new List<User>();
                users = await chatService.LoginAsync(_userName, Avatar());
                if (users != null)
                {
                    users.ForEach(u => Participants.Add(new Participant { Name = u.Name, Photo = u.Photo, Score = 0 }));
                    UserMode = UserModes.Chat;
                    IsLoggedIn = true;
                    return true;
                }
                else
                {
                    dialogService.ShowNotification("Username is already in use");
                    return false;
                }

            }
            catch (Exception) { return false; }
        }

        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(UserName) && UserName.Length >= 2 && IsConnected;
        }
        #endregion

        #region Logout Command
        private ICommand _logoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                return _logoutCommand ?? (_logoutCommand =
                    new RelayCommandAsync(() => Logout(), (o) => CanLogout(), (ex) => NotiMsg = ex.Message));
            }
        }

        private async Task<bool> Logout()
        {
            try
            {
                await chatService.LogoutAsync();
                UserMode = UserModes.Login;
                //화면바꾸는 로직

                return true;
            }
            catch (Exception) { return false; }
        }

        private bool CanLogout()
        {
            return IsConnected && IsLoggedIn;
        }
        #endregion

        #region Typing Command
        private ICommand _typingCommand;
        public ICommand TypingCommand
        {
            get
            {
                return _typingCommand ?? (_typingCommand =
                    new RelayCommandAsync(() => Typing(), (o) => CanUseTypingCommand(), (ex) => NotiMsg = ex.Message));
            }
        }

        private async Task<bool> Typing()
        {
            try
            {
                await chatService.TypingAsync(UserName);
                return true;
            }
            catch (Exception) { return false; }
        }

        private bool CanUseTypingCommand()
        {
            return (SelectedParticipant != null && SelectedParticipant.IsLoggedIn);
        }
        #endregion

        #region Send Text Message Command
        private ICommand _sendTextMessageCommand;
        public ICommand SendTextMessageCommand
        {
            get
            {
                return _sendTextMessageCommand ?? (_sendTextMessageCommand =
                    new RelayCommandAsync(() => SendTextMessage(), (o) => CanSendTextMessage(), (ex) => NotiMsg = ex.Message));
            }
        }

        private async Task<bool> SendTextMessage()
        {
            try
            {
                //var recepient = _selectedParticipant.Name;
                await chatService.SendBroadcastMessageAsync(_textMessage);
                if (!IsHost && IsInGame)
                {
                    await chatService.SendAnswerAsync(_textMessage);
                }
                return true;
            }
            catch (Exception) { return false; }
            finally
            {
                ChatMessage msg = new ChatMessage
                {
                    Author = UserName,
                    Message = _textMessage,
                    Time = DateTime.Now,
                    IsOriginNative = true
                };
                ChatLog.Add(msg);
                TextMessage = string.Empty;
            }
        }

        private bool CanSendTextMessage()
        {
            return (!string.IsNullOrEmpty(TextMessage) && IsConnected);
        }
        #endregion

        #region Send Picture Message Command
        private ICommand _sendImageMessageCommand;
        public ICommand SendImageMessageCommand
        {
            get
            {
                return _sendImageMessageCommand ?? (_sendImageMessageCommand =
                    new RelayCommandAsync(() => SendImageMessage(), (o) => CanSendImageMessage(), (ex) => NotiMsg = ex.Message));
            }
        }

        private async Task<bool> SendImageMessage()
        {
            var pic = dialogService.OpenFile("Select image file", "Images (*.jpg;*.png)|*.jpg;*.png");
            if (string.IsNullOrEmpty(pic)) return false;

            var img = await Task.Run(() => File.ReadAllBytes(pic));

            try
            {
                var recepient = _selectedParticipant.Name;
                await chatService.SendBroadcastMessageAsync(img);
                return true;
            }
            catch (Exception) { return false; }
            finally
            {
                ChatMessage msg = new ChatMessage { Author = UserName, Picture = pic, Time = DateTime.Now, IsOriginNative = true };
                ChatLog.Add(msg);
            }           
        }

        private bool CanSendImageMessage()
        {
            return IsConnected;
        }
        #endregion

        #region Select Profile Picture Command
        private ICommand _selectProfilePicCommand;
        public ICommand SelectProfilePicCommand
        {
            get
            {
                return _selectProfilePicCommand ?? (_selectProfilePicCommand =
                    new RelayCommand((o) => SelectProfilePic()));
            }
        }

        private void SelectProfilePic()
        {
            var pic = dialogService.OpenFile("Select image file", "Images (*.jpg;*.png)|*.jpg;*.png");
            if (!string.IsNullOrEmpty(pic))
            {
                var img = Image.FromFile(pic);
                if (img.Width > MAX_IMAGE_WIDTH || img.Height > MAX_IMAGE_HEIGHT)
                {
                    dialogService.ShowNotification($"Image size should be {MAX_IMAGE_WIDTH} x {MAX_IMAGE_HEIGHT} or less.");
                    return;
                }
                ProfilePic = pic;
            }
        }
        #endregion

        #region Stroke Command
        private ICommand _StrokeCanvasCommand;

        public ICommand StrokeCanvasCommand
        {
            get
            {
                return _StrokeCanvasCommand ?? (_StrokeCanvasCommand =
                    new RelayCommandAsync(() => StrokeCanvas(), (o)=> CanStrokeCanvas(), (ex) => NotiMsg = ex.Message));
            }
        }

        private bool CanStrokeCanvas()
        {
            return IsLoggedIn && IsHost && IsInGame ;
            //return true;
        }

        private async Task<bool> StrokeCanvas()
        {

            using (MemoryStream memoryStream = new MemoryStream()) 
            {
                Strokes.Save(memoryStream);
            
                try
                {
                    //var recepient = _selectedParticipant.Name;
                    await chatService.SendBroadcastStrokesAsync(memoryStream.ToArray());
                    return true;
                }
                catch (Exception) { return false; }
            }
        }

        #endregion

        #region Open Image Command
        private ICommand _openImageCommand;
        public ICommand OpenImageCommand
        {
            get
            {
                return _openImageCommand ?? (_openImageCommand =
                    new RelayCommand<ChatMessage>((m) => OpenImage(m)));
            }
        }

        private void OpenImage(ChatMessage msg)
        {
            var img = msg.Picture;
            if (string.IsNullOrEmpty(img) || !File.Exists(img)) return;
            Process.Start(img);
        }
        #endregion


        #region Event Handlers

        private void GameStart(string noticeMsg, MessageType mt)
        {
            if( IsLoggedIn && mt == MessageType.Broadcast)
            {
                IsInGame = true;
                // 배경음
                //MainSound = "C:\\Users\\user\\Downloads\\SignalChat-master\\ChatClientCS\\Audios\\Main.mp4";
                MainSound = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Audios\\Main.mp4";

                // 게임 시작 알림하기
                //Observable.Timer(TimeSpan.FromMilliseconds(3000)).Subscribe(t => 
                //ctxTaskFactory.StartNew(() => MessageBox.Show(noticeMsg)).Wait()
                //).Dispose();
                NotiMsg = noticeMsg + CanBeginGame().ToString();
                Observable.Timer(TimeSpan.FromMilliseconds(3000)).Subscribe(t => NotiMsg = "");
            }
        }

        private void GameEnd(string noticeMsg, MessageType mt)
        {
            if (IsLoggedIn && mt == MessageType.Broadcast)
            {
                IsInGame = false;
                CurrentAnswer = "";
                // 점수 초기화
                foreach(Participant p in Participants)
                {
                    p.Score = 0;
                }

                // 배경음
                MainSound = "";
                Effect = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Audios\\result.mp4";

                // 게임 끝 알림하기
                NotiMsg = noticeMsg + CanEndGame().ToString();
                Observable.Timer(TimeSpan.FromMilliseconds(3000)).Subscribe(t => NotiMsg = "");
            }
        }

        private void SetNewHost(string host, string nextAnswer, MessageType mt)
        {
            Application.Current.Dispatcher.InvokeAsync(() => Strokes.Clear());
            if (host == UserName) IsHost = true;
            else IsHost = false;
            
            if (IsHost && IsInGame) CurrentAnswer = nextAnswer;
            else
            {
                string blank = "";
                for(int i=0; i < nextAnswer.Length; i++)
                {
                    blank += "□";
                }
                CurrentAnswer = blank;
            }

            
        }

        private void NewTextMessage(string name, string msg, MessageType mt)
        {
            if (mt == MessageType.Unicast)
            {
                ChatMessage cm = new ChatMessage { Author = name, Message = msg, Time = DateTime.Now };
                var sender = _participants.Where((u) => string.Equals(u.Name, name)).FirstOrDefault();
                ctxTaskFactory.StartNew(() => sender.Chatter.Add(cm)).Wait();

                if (!(SelectedParticipant != null && sender.Name.Equals(SelectedParticipant.Name)))
                {
                    ctxTaskFactory.StartNew(() => sender.HasSentNewMessage = true).Wait();
                }
            }
            else if (mt == MessageType.Broadcast)
            {
                ChatMessage cm = new ChatMessage { Author = name, Message = msg, Time = DateTime.Now, IsOriginNative = false };
                ctxTaskFactory.StartNew(() => ChatLog.Add(cm)).Wait();
                //ctxTaskFactory.StartNew(() => sender.HasSentNewMessage = true).Wait();
            }
        }

        private void NewImageMessage(string name, byte[] pic, MessageType mt)
        {
            if (mt == MessageType.Unicast)
            {
                var imgsDirectory = Path.Combine(Environment.CurrentDirectory, "Image Messages");
                if (!Directory.Exists(imgsDirectory)) Directory.CreateDirectory(imgsDirectory);

                var imgsCount = Directory.EnumerateFiles(imgsDirectory).Count() + 1;
                var imgPath = Path.Combine(imgsDirectory, $"IMG_{imgsCount}.jpg");

                ImageConverter converter = new ImageConverter();
                using (Image img = (Image)converter.ConvertFrom(pic))
                {
                    img.Save(imgPath);
                }

                ChatMessage cm = new ChatMessage { Author = name, Picture = imgPath, Time = DateTime.Now };
                var sender = _participants.Where(u => string.Equals(u.Name, name)).FirstOrDefault();
                ctxTaskFactory.StartNew(() => sender.Chatter.Add(cm)).Wait();

                if (!(SelectedParticipant != null && sender.Name.Equals(SelectedParticipant.Name)))
                {
                    ctxTaskFactory.StartNew(() => sender.HasSentNewMessage = true).Wait();
                }
            }
        }
        private void NewStrokesCollected(string name, byte[] strokesArr, MessageType mt)
        {
            if (_isLoggedIn && mt == MessageType.Broadcast)
            {
                Stream stream = new MemoryStream(strokesArr);
                ctxTaskFactory.StartNew(() => Strokes = new StrokeCollection(stream)).Wait();
            }
        }

        private void ParticipantLogin(User u)
        {
            var ptp = Participants.FirstOrDefault(p => string.Equals(p.Name, u.Name));
            if (_isLoggedIn && ptp == null)
            {
                Effect = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Audios\\Login.mp4";
                Observable.Timer(TimeSpan.FromMilliseconds(2000)).Subscribe(t => Effect = "");
                ctxTaskFactory.StartNew(() => Participants.Add(new Participant
                {
                    Name = u.Name,
                    Photo = u.Photo
                })).Wait();
            }
        }

        private void ParticipantDisconnection(string name)
        {
            var person = Participants.Where((p) => string.Equals(p.Name, name)).FirstOrDefault();
            if (_isLoggedIn && person != null)
            {
                Effect = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Audios\\Logout.mp4";

                Observable.Timer(TimeSpan.FromMilliseconds(2000)).Subscribe(t => Effect = "");
                // using은 자원을 할당할 경우만 쓰시오.
                Application.Current.Dispatcher.InvokeAsync(() => Participants.Remove(person));
                //if (person != null) person.IsLoggedIn = false;
            }
        }

        private void ParticipantReconnection(string name)
        {
            var person = Participants.Where((p) => string.Equals(p.Name, name)).FirstOrDefault();
            if (person != null) person.IsLoggedIn = true;
        }

        private void Reconnecting()
        {
            IsConnected = false;
            IsLoggedIn = false;
        }

        private async void Reconnected()
        {
            var pic = Avatar();
            if (!string.IsNullOrEmpty(_userName)) await chatService.LoginAsync(_userName, pic);
            IsConnected = true;
            IsLoggedIn = true;
        }

        private async void Disconnected()
        {
            var connectionTask = chatService.ConnectAsync();
            await connectionTask.ContinueWith(t => {
                if (!t.IsFaulted)
                {
                    IsConnected = true;
                    chatService.LoginAsync(_userName, Avatar()).Wait();
                    IsLoggedIn = true;
                }
            });
        }

        private void ParticipantTyping(string name)
        {
            var person = Participants.Where((p) => string.Equals(p.Name, name)).FirstOrDefault();
            if (person != null && !person.IsTyping)
            {
                person.IsTyping = true;
                Observable.Timer(TimeSpan.FromMilliseconds(1500)).Subscribe(t => person.IsTyping = false);
            }
        }

        private void Correct(string notiMsg, string sender)
        {

            var person = Participants.Where((p) => string.Equals(p.Name, sender)).FirstOrDefault();
            if(person != null)
            {
                Right = $"{sender}님 정답";
                person.Score++;
                Effect = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Audios\\CorrectAnswer.mp4";
                NotiMsg = notiMsg;

                Observable.Timer(TimeSpan.FromMilliseconds(3000)).Subscribe(t => Effect = "");
                Observable.Timer(TimeSpan.FromMilliseconds(3000)).Subscribe(t => NotiMsg = "") ;
            }

        }
        #endregion

        private byte[] Avatar()
        {
            byte[] pic = null;
            if (!string.IsNullOrEmpty(_profilePic)) pic = File.ReadAllBytes(_profilePic);
            return pic;
        }

        public MainWindowViewModel(IChatService chatSvc, IDialogService diagSvc)
        {
            dialogService = diagSvc;
            chatService = chatSvc;

            chatSvc.GameStart += GameStart;
            chatSvc.GameEnd += GameEnd;
            chatSvc.SetNewHost += SetNewHost;
            chatSvc.NewTextMessage += NewTextMessage;
            chatSvc.NewImageMessage += NewImageMessage;
            chatSvc.NewStrokesCollected += NewStrokesCollected;
            chatSvc.ParticipantLoggedIn += ParticipantLogin;
            chatSvc.ParticipantLoggedOut += ParticipantDisconnection;
            chatSvc.ParticipantDisconnected += ParticipantDisconnection;
            chatSvc.ParticipantReconnected += ParticipantReconnection;
            chatSvc.ParticipantTyping += ParticipantTyping;
            chatSvc.ConnectionReconnecting += Reconnecting;
            chatSvc.ConnectionReconnected += Reconnected;
            chatSvc.ConnectionClosed += Disconnected;
            chatSvc.Correct += Correct;

            ctxTaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
        }

    }
}