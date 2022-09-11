using System.Net;
using System.Text;
using lib;

namespace client;

public abstract class Message
{
    public enum EMessageType
    {
        Play,
        Start
    }

    public EMessageType MessageType;
    public static List<Message> Parse(string data)
    {
        var messages = new List<Message>();
        var parts = data.Split(EOM, StringSplitOptions.RemoveEmptyEntries);
        foreach (var messageData in parts)
        {
            var message = decode(messageData);
            if (message != null)
            {
                messages.Add(message);
            }
        }

        return messages;
    }

    private static Message? decode(string messageData)
    {
        string[] parts = messageData.Split(DELIM, StringSplitOptions.RemoveEmptyEntries);
        if (EMessageType.TryParse(parts[0], true, out EMessageType messageType))
        {
            switch (messageType)
            {
                case EMessageType.Start:
                    return StartMessage.FromNet(parts);
                case EMessageType.Play:
                    return PlayMessage.FromNet(parts);
            }
        }

        throw new ProtocolViolationException();
    }

    private const char DELIM = ',';

    private const char EOM = '|';

    public static Message Start(string? name, string? seed)
    {
        return new StartMessage(name, seed);
    }

    public byte[] ToNet()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("{0},", MessageType.ToString());
        sb.AppendFormat("{0},", detailsToString());
        sb.Append(EOM);
        return System.Text.Encoding.ASCII.GetBytes(sb.ToString());
    }

    protected abstract string? detailsToString();

    public static PlayMessage? Play(uint card, string[]? data)
    {
        return PlayMessage.Create(card, data);
    }
}

public class PlayMessage : Message
{
    public uint Card { get; private set; }
    public string[]? Data { get; private set; }

    private PlayMessage()
    {
    }

    protected override string? detailsToString()
    {
        if (Data != null) return $"{Card.ToString()},{string.Join(',', Data)}";
        throw new ProtocolViolationException();
    }

    public static PlayMessage? Create(uint card, string[]? data)
    {
        return new PlayMessage()
        {
            Card = card,
            Data = data
        };
    }

    public static Message? FromNet(string[]? parts)
    {
        if (parts != null && parts.Length > 2)
        {
            var extraData = parts.Skip(2).Take(parts.Length - 2).ToArray();
            return Create(UInt32.Parse(parts[1]), extraData);
        }

        throw new ProtocolViolationException();
    }
}
