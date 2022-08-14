namespace client;

public class Message
{
    public enum EMessageType
    {
        Done,
        Quit,
        Start
    }

    public EMessageType MessageType;
    public static List<Message> Parse(string data)
    {
        var messages = new List<Message>();
        var parts = data.Split(EOM);
        foreach (var messageData in parts)
        {
            var message = decode(messageData);
            messages.Add(message);
        }

        return messages;
    }

    private static Message decode(string messageData)
    {
        return new Message();
    }

    private const char EOM = '|';

    public static Message Done()
    {
        return new Message() { MessageType = EMessageType.Done };
    }
    
    public static Message Start()
    {
        return new Message() { MessageType = EMessageType.Start };
    }
}