using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.AspNet.SignalR;

namespace ChatServerCS
{
    public class ChatHub : Hub<IClient>
    {
        private static int GameCount = 0;
        private static bool InGame = false;
        private static User CurrentHost;
        private static QuizAnswerRepo AnswerList;

        private static ConcurrentDictionary<string, User> UserList = new ConcurrentDictionary<string, User>();
        //private static ConcurrentDictionary<string, Game> GameList = new ConcurrentDictionary<string, Game>();

        public override Task OnDisconnected(bool stopCalled)
        {
            var userName = UserList.SingleOrDefault((c) => c.Value.ID == Context.ConnectionId).Key;
            if (userName != null)
            {
                Clients.Others.ParticipantDisconnection(userName);
                if(UserList.Count < 2 && InGame)
                {
                    string notiMsg = "플레이어의 숫자가 적어 게임이 종료되었습니다.";
                    Clients.All.BroadcastGameEnd(notiMsg);
                    GameCount = 0;
                    InGame = false;

                    Console.WriteLine($"-- Game Ended");
                }
                Console.WriteLine($"<> {userName} disconnected");
            }
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            var userName = UserList.SingleOrDefault((c) => c.Value.ID == Context.ConnectionId).Key;
            if (userName != null)
            {
                Clients.Others.ParticipantReconnection(userName);
                Console.WriteLine($"== {userName} reconnected");
            }
            return base.OnReconnected();
        }

        public List<User> Login(string name, byte[] photo)
        {
            if (!UserList.ContainsKey(name))
            {
                Console.WriteLine($"++ {name} logged in");
                User newUser = new User { Name = name, ID = Context.ConnectionId, Photo = photo, seq = UserList.Count + 1 };
                var added = UserList.TryAdd(name, newUser);
                List<User> users = new List<User>(UserList.Values);
                if (!added) return null;
                Clients.CallerState.UserName = name;
                Clients.Others.ParticipantLogin(newUser);

                if (InGame)
                {
                    string notiMsg = $"현재 게임 중입니다. 현재 출제자는 {CurrentHost.Name}님 입니다.";
                    Clients.Client(newUser.ID).BroadcastGameStart(notiMsg);
                    Clients.Client(newUser.ID).BroadcastSetHost(CurrentHost.Name, AnswerList.CurrentAnswer);

                }

                return users;
            }
            return null;
        }


        //// 이거 만들다 맘 ConcurrentDictionary<int, game>을 만들어야되나?
        //public List<User> CreateGameRoom(string UserName, byte[] photo, string GameName)
        //{
        //    if (!UserList.ContainsKey(UserName))
        //    {
        //        Console.WriteLine($"++ {UserName} create game titled : {GameName} ");
        //        int newGameNumber = GameList.Count + 1;
        //        List<User> users = new List<User>(UserList.Values);
        //        Game newGame = new Game { Name = GameName, Number = newGameNumber };
        //        User newUser = new User { Name = UserName, ID = Context.ConnectionId, Photo = photo };
        //        var added = GameList.TryAdd(GameName, newGame);
        //        if (!added) return null;
        //        Groups.Add(Context.ConnectionId, GameName);
        //        Clients.CallerState.UserName = UserName;
        //        Clients.Others.ParticipantLogin(newUser);
        //        return users;
        //    }
        //    return null;
        //}

        public void Logout()
        {
            var name = Clients.CallerState.UserName;
            if (!string.IsNullOrEmpty(name))
            {
                User client = new User();
                UserList.TryRemove(name, out client);
                Clients.Others.ParticipantLogout(name);
                
                if (UserList.Count < 2 && InGame)
                {
                    string notiMsg = "플레이어의 숫자가 적어 게임이 종료되었습니다.";
                    Clients.All.BroadcastGameEnd(notiMsg);
                    GameCount = 0;
                    InGame = false;

                    Console.WriteLine($"-- Game Ended");
                }

                Console.WriteLine($"-- {name} logged out");
            }
        }

        public void BeginGame()
        {
            var name = Clients.CallerState.UserName;
            if (!string.IsNullOrEmpty(name) && !InGame)
            {
                InGame = true;

                //User host = new User();
                //CurrentHost = UserList.Values.SingleOrDefault((u) => u.seq == 1);
                int minSeq = UserList.Min((u) => u.Value.seq);
                CurrentHost = UserList.Values.SingleOrDefault((u) => u.seq == minSeq);
                AnswerList = new QuizAnswerRepo();
                string noticeMsg = $"{name}님이 게임을 시작했습니다. 출제자는 {CurrentHost.Name}님 입니다.";
                Clients.All.BroadcastGameStart(noticeMsg);
                Clients.All.BroadcastSetHost(CurrentHost.Name, AnswerList.GenerateAnwer());

                Console.WriteLine($"-- {name} started the game / ingame : {InGame.ToString()}");
            }
        }

        public void EndGame()
        {
            var name = Clients.CallerState.UserName;
            if (!string.IsNullOrEmpty(name) && InGame)
            {
                InGame = false;
                GameCount = 0;
                string notiMsg = "게임이 종료되었습니다.";
                Clients.All.BroadcastGameEnd(notiMsg);
                Console.WriteLine($"-- Game Ended / ingmae : {InGame.ToString()}");
            }
        }

        public void BroadcastTextMessage(string message)
        {
            var name = Clients.CallerState.UserName;
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(message))
            {
                Clients.Others.BroadcastTextMessage(name, message);
            }
        }

        public void BroadcastImageMessage(byte[] img)
        {
            var name = Clients.CallerState.UserName;
            if (img != null)
            {
                Clients.Others.BroadcastPictureMessage(name, img);
            }
        }

        public void BroadcastStrokes(byte[] strokeArr)
        {
            var name = Clients.CallerState.UserName;
            if (strokeArr != null)
            {
                Clients.Others.BroadcastStrokes(name, strokeArr);
            }
        }

        public void UnicastTextMessage(string recepient, string message)
        {
            var sender = Clients.CallerState.UserName;
            if (!string.IsNullOrEmpty(sender) && recepient != sender &&
                !string.IsNullOrEmpty(message) && UserList.ContainsKey(recepient))
            {
                User client = new User();
                UserList.TryGetValue(recepient, out client);
                Clients.Client(client.ID).UnicastTextMessage(sender, message);
            }
        }

        public void UnicastImageMessage(string recepient, byte[] img)
        {
            var sender = Clients.CallerState.UserName;
            if (!string.IsNullOrEmpty(sender) && recepient != sender &&
                img != null && UserList.ContainsKey(recepient))
            {
                User client = new User();
                UserList.TryGetValue(recepient, out client);
                Clients.Client(client.ID).UnicastPictureMessage(sender, img);
            }
        }

        public void Typing(string recepient)
        {
            if (string.IsNullOrEmpty(recepient)) return;
            var sender = Clients.CallerState.UserName;
            User client = new User();
            UserList.TryGetValue(recepient, out client);
            Clients.Client(client.ID).ParticipantTyping(sender);
        }

        public void IsRight(string message)
        {

            var sender = Clients.CallerState.UserName;

            if (!string.IsNullOrEmpty(sender) && !string.IsNullOrEmpty(message))
            {
                // 정답 이벤트 송출
                if (InGame && message.Equals(AnswerList.CurrentAnswer)) {
                    Console.WriteLine($"{sender} solves the quiz!");
                    string notiMsg = $"{sender}님이 정답을 맞추셧습니다!";
                    Clients.All.BraodcastAnswerIsRight(notiMsg ,sender);
                }

                // 다음 턴 세팅
                if (GameCount <= 10 && InGame)
                {
                    //User oldHost = new User();
                    //UserList.TryGetValue(sender,out oldHost);
                    //User newHost;
                    if(UserList.Count >= CurrentHost.seq + 1)
                    {
                        for(int i = CurrentHost.seq + 1; i <= UserList.Count; i++)
                        {
                            
                            var userName = UserList.SingleOrDefault((c) => c.Value.seq == i).Key;
                            if (userName != null)
                            {
                                CurrentHost = UserList.Values.SingleOrDefault((c) => c.seq == i);
                                break;
                            }
                        }
                        Console.WriteLine($"seq : {CurrentHost.seq}");
                    }
                    else
                    {
                        CurrentHost = UserList.Values.SingleOrDefault<User>((u) => u.seq == 1);
                        Console.WriteLine($"seq : {CurrentHost.seq}");

                    }

                    Clients.All.BroadcastSetHost(CurrentHost.Name, AnswerList.GenerateAnwer());

                    GameCount++;
                }
                else
                {
                    string notiMsg = "게임이 종료되었습니다.";
                    Clients.All.BroadcastGameEnd(notiMsg);

                    GameCount = 0;
                    InGame = false;
                }
            }
        }
    }
}