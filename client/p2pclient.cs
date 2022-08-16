using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text.RegularExpressions;

namespace client;

public class p2pclient
{
    private TcpClient client;
    private string name;
    private NetworkStream stream;
    private Queue<Message> messageQueue = new Queue<Message>();

    public void Connect(string name)
    {
        this.name = name;
        var thread = new Thread(doClient);
        thread.Start();
    }

    private void doClient()
    {
        client = new TcpClient(Address, Port);
        Console.WriteLine("Connected to server");
        stream = client.GetStream();
        startGame(NetGame.EPlayerType.Challenger);
    }

    private const string Address = "127.0.0.1";

    private const int Port = 5309;

    public void Host(string name)
    {
        this.name = name;
        var thread = new Thread(doHost);
        thread.Start();
    }

    private void doHost()
    {
        var server = new TcpListener(IPAddress.Parse(Address), Port);
        server.Start();
        
        Console.WriteLine("Hosting on 127.0.0.1:5309");
        
        client = server.AcceptTcpClient();
        Console.WriteLine("Accepted Connection");
        stream = client.GetStream();

        startGame(NetGame.EPlayerType.Host);
    }

    private void startGame(NetGame.EPlayerType playerType)
    {
        var details = new PlayerDetails()
        {
            Name = name,
            Seed = ((int)(DateTime.Now.Ticks & 0x0000ffff) ^ name.GetHashCode())
        };

        var challengerDetails = introduction(details);

        NetGame netGame = playerType == NetGame.EPlayerType.Host
            ? NetGame.Host(details, challengerDetails)
            : NetGame.Challenge(details, challengerDetails);
        playGame(netGame);
    }

    private PlayerDetails introduction(PlayerDetails details)
    {
        var start = Message.Start(details.Name, details.Seed.ToString());
        stream.Write(start.ToNet());
        var message = getMessage() as StartMessage;
        if (message == null)
        {
            throw new ProtocolViolationException();
        }

        return new PlayerDetails() { Name = message.Name, Seed = message.Seed };
    }

    private Message getMessage()
    {
        if (messageQueue.Count == 0)
        {
            waitForMessages();
        }
        return messageQueue.Dequeue();
    }

    private void waitForMessages()
    {
        byte[] bytes = new byte[256];
        var receivedCount = stream.Read(bytes, 0, bytes.Length);
        while (receivedCount != 0)
        {
            Console.WriteLine("Received {0} bytes", receivedCount);
            var data = System.Text.Encoding.ASCII.GetString(bytes, 0, receivedCount);
            var messages = Message.Parse(data);
            Console.WriteLine("Found {0} message", messages.Count);
            foreach (var message in messages)
            {
                messageQueue.Enqueue(message);
            }

            if (client.Available > 0)
            {
                receivedCount = stream.Read(bytes, 0, bytes.Length);
            }
            else
            {
                receivedCount = 0;
            }
        }
    }

    private void playGame(NetGame netGame)
    {
        Console.WriteLine("Playing In Gladio");
        Console.WriteLine($"{netGame.HostName} challenging {netGame.ChallengerName}");
        while (!netGame.Closed)
        {
            Console.WriteLine($"Game is open");

            var turn = netGame.YourTurn();
            sendMessage(turn.Message);

            Console.WriteLine("Waiting for opponent to play");
            waitForMessages();

            var opponentsTurn = new Turn()
            {
                Message = messageQueue.Dequeue() as PlayMessage,
                Local = false
            };

            netGame.UpdateGame(turn, opponentsTurn);
        }
    }

    private void sendMessage(Message message)
    {
        stream.Write(message.ToNet());
    }
}