namespace client;

public interface InGladioNetwork
{
    Queue<Message> MessageQueue { get; }
    void WaitForMessages();
    void SendMessage(PlayMessage turn);
}