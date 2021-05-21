namespace ChatServerCS
{
    public interface IClient
    {
        void ParticipantDisconnection(string name);
        void ParticipantReconnection(string name);
        void ParticipantLogin(User client);
        void ParticipantLogout(string name);
        void BroadcastGameStart(string notiMsg);
        void BroadcastGameEnd(string notiMsg);
        void BroadcastSetHost(string hostName, string answer);
        void BroadcastTextMessage(string sender, string message);
        void BroadcastPictureMessage(string sender, byte[] img);
        void BroadcastStrokes(string sender, byte[] img);
        void BraodcastAnswerIsRight(string msg,string sender);
        void UnicastTextMessage(string sender, string message);
        void UnicastPictureMessage(string sender, byte[] img);
        void UnicastSetNextGame(string sender);
        void ParticipantTyping(string sender);

    }
}