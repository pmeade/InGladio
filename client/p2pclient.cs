using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace client;

public class p2pclient
{
    public p2pclient()
    {
    }

    public void Connect()
    {
        var thread = new Thread(doClient);
        thread.Start();
    }

    private void doClient()
    {
        TcpClient client = new TcpClient(Address, Port);
        Console.WriteLine("Connected to server");
        var stream = client.GetStream();
        var start = Message.Start();
        stream.Write(System.Text.Encoding.ASCII.GetBytes(start.ToString()));
        playGame(stream);
    }

    private const string Address = "127.0.0.1";

    private const int Port = 5309;

    public void Host()
    {
        var thread = new Thread(doHost);
        thread.Start();
    }

    private void doHost()
    {
        var server = new TcpListener(IPAddress.Parse(Address), Port);
        server.Start();
        
        Console.WriteLine("Hosting on 127.0.0.1:5309");
        
        
        var client = server.AcceptTcpClient();
        Console.WriteLine("Accepted Connection");
        var stream = client.GetStream();

        playGame(stream);

    }

    private void playGame(NetworkStream stream)
    {
        Game game = new Game();

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
                game.Handle(message);
            }

            var reponse = game.YourTurn();

            foreach (var message in reponse)
            {
                var outbytes = System.Text.Encoding.ASCII.GetBytes(message.ToString());
                stream.Write(outbytes);
            }

            if (game.Closed)
            {
                break;
            }
            
            receivedCount = stream.Read(bytes, 0, bytes.Length);
        }
    }
}