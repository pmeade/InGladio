﻿using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text.RegularExpressions;

namespace client;

public class p2pclient : InGladioNetwork
{
    private TcpClient? client;
    private string? name;
    private NetworkStream? stream;
    private readonly Queue<Message> messageQueue = new Queue<Message>();
    private PlayerDetails? localPlayerdetails;
    private PlayerDetails? opponentDetails;

    public void Connect(string? name)
    {
        this.name = name ?? throw new ArgumentNullException(nameof(name));
        doClient();
    }

    private void doClient()
    {
        client = new TcpClient(Address, Port);
        Console.WriteLine("Connected to server");
        stream = client.GetStream();
        startGame(NetGame.EPlayerType.Challenger);
        stream.Close();
        client.Close();
    }

    private const string Address = "127.0.0.1";

    private const int Port = 5309;

    public void Host(string? name)
    {
        this.name = name;
        doHost();
    }

    private void doHost()
    {
        var server = new TcpListener(IPAddress.Parse(Address), Port);
        server.Start();
        
        Console.WriteLine("");
        Console.WriteLine("Hosting on 127.0.0.1:5309");
        
        client = server.AcceptTcpClient();
        Console.WriteLine("Accepted Connection");
        Console.WriteLine("");
        stream = client.GetStream();

        startGame(NetGame.EPlayerType.Host);
        stream.Close();
        client.Close();
        server.Stop();
    }

    private void startGame(NetGame.EPlayerType playerType)
    {
        Console.WriteLine("");
        if (name != null)
            localPlayerdetails = new PlayerDetails()
            {
                Name = name,
                Seed = ((int)(DateTime.Now.Ticks & 0x0000ffff) ^ name.GetHashCode())
            };

        opponentDetails = introduction(localPlayerdetails);

        NetGame? netGame = playerType == NetGame.EPlayerType.Host
            ? NetGame.Host(localPlayerdetails, opponentDetails)
            : NetGame.Challenge(localPlayerdetails, opponentDetails);
        
        netGame?.Start();
        
        Console.WriteLine("");
        Console.WriteLine("Playing In Gladio");
        Console.WriteLine("");
        netGame?.Tick(this);
    }

    private PlayerDetails? introduction(PlayerDetails? details)
    {
        var start = Message.Start(details?.Name, details?.Seed.ToString());
        stream?.Write(start.ToNet());
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
        if (stream != null)
        {
            var receivedCount = stream.Read(bytes, 0, bytes.Length);
            while (receivedCount != 0)
            {
                //Console.WriteLine("Received {0} bytes", receivedCount);
                var data = System.Text.Encoding.ASCII.GetString(bytes, 0, receivedCount);
                var messages = Message.Parse(data);
                //Console.WriteLine("Found {0} message", messages.Count);
                foreach (var message in messages)
                {
                    messageQueue.Enqueue(message);
                }

                if (client != null && client.Available > 0)
                {
                    receivedCount = stream.Read(bytes, 0, bytes.Length);
                }
                else
                {
                    receivedCount = 0;
                }
            }
        }
    }


    private void sendMessage(Message? message) => stream?.Write(message?.ToNet());
    public Queue<Message> MessageQueue => messageQueue;
    public void WaitForMessages() => waitForMessages();
    public void SendMessage(PlayMessage turn) => sendMessage(turn);
}