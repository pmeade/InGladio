using System.Text;
using lib;


namespace client;

public class NetGame
{
    enum EGameState
    {
        Init,
        Done
    }

    private int round = 0;

    private Turn takeTurn()
    {
        displayGame();

        var cardIndex = makePlay();
        var card = getLocalCard(cardIndex);
        return getAdditionalInput(card, cardIndex);
    }

    private Turn getAdditionalInput(Card? card, uint cardIndex)
    {
        switch (card?.Choice)
        {
            case Choice.Move:
                return completeMove(cardIndex);

            case Choice.Strike:
                return completeStrike(cardIndex);

            case Choice.Parry:
            default:
                return completeParry(cardIndex);
        }
    }

    private Card? getLocalCard(uint cardIndex)
    {
        return _getLocalPlayer()?.Deck.Get(cardIndex);
    }

    private uint makePlay()
    {
        var key = ConsoleKey.Clear;
        key = chooseCardToPlay(key);

        var cardIndex = (uint)(key - ConsoleKey.D1);
        return cardIndex;
    }

    private Turn completeParry(uint cardIndex)
    {
        var target = inputTarget();
        string[] data = new[] { target ?? string.Empty};
        return new Turn()
        {
            Message = Message.Play(cardIndex, data),
            Local = true
        };
    }

    private Turn completeStrike(uint cardIndex)
    {
        var target = inputTarget();
        string[] data = new[] { target ?? string.Empty};
        return new Turn()
        {
            Message = Message.Play(cardIndex, data),
            Local = true
        };
    }

    private Turn completeMove(uint cardIndex)
    {
        var moveLocation = inputLocation();
        var mover = inputMover();
        var data = new[] { moveLocation ?? string.Empty, mover ?? String.Empty };
        return new Turn()
        {
            Message = Message.Play(cardIndex, data),
            Local = true
        };
    }

    private ConsoleKey chooseCardToPlay(ConsoleKey key)
    {
        while (key < ConsoleKey.D1 || key > ConsoleKey.D8)
        {
            var input = Console.ReadKey();
            key = input.Key;
            Console.WriteLine();
            if (_burned.ContainsKey(key))
            {
                key = ConsoleKey.Clear;
            }
        }

        return key;
    }

    private string? inputMover()
    {
        if (_getLocalPlayer()?.Location != match?.Basket.Location)
        {
            return "self";
        }

        var key = TextInput.Get(new Dictionary<ConsoleKey, string>()
        {
            {
                ConsoleKey.S,
                "Self"
            },
            {
                ConsoleKey.B,
                "Basket"
            }
        });

        switch (key)
        {
            case ConsoleKey.B:
                return "basket";
            default:
            case ConsoleKey.S:
                return "self";
        }

    }
    
    private string? inputTarget()
    {
        if (_getLocalPlayer()?.Location != match?.Basket.Location)
        {
            return "Opponent";
        }

        
        var key = TextInput.Get(new Dictionary<ConsoleKey, string>()
        {
            {
                ConsoleKey.O,
                "Opponent"
            },
            {
                ConsoleKey.B,
                "Basket"
            }
        });

        switch (key)
        {
            case ConsoleKey.B:
                return "basket";
            default:
            case ConsoleKey.O:
                return "opponent";
        }
    }

    private string? inputLocation()
    {
        var choices = new Dictionary<ConsoleKey, string>();
        switch (_getLocalPlayer()?.Place)
        {
            case Place.Curtain:
                return "Square";
            case Place.Perch:
                return "Steps";
            case Place.Steps:
                choices.Add(ConsoleKey.Q, "Square");
                choices.Add(ConsoleKey.P, "Perch");
                break;
            case Place.Square:
                choices.Add(ConsoleKey.C, "Curtain");
                choices.Add(ConsoleKey.S, "Steps");
                break;
        }
        
        var key = TextInput.Get(choices);

        switch (key)
        {
            case ConsoleKey.Q:
                return "Square";
            case ConsoleKey.P:
                return "Perch";
            case ConsoleKey.C:
                return "Curtain";
            default:
                return "Steps";
        }
    }
    private void displayBasket()
    {
        Console.WriteLine();
        Console.WriteLine($"Basket: Health {match?.Basket.Health} {match?.Basket.Face.ToPrettyString()}");
    }


    private void displayGame()
    {
        Console.WriteLine();
        Console.WriteLine("*** ***** ***");
        Console.WriteLine("Round {0}", ++round);
        displayOpponent();
        displayBasket();
        displayLocalPlayer();
    }

    private void displayLocalPlayer()
    {
        var you = _getLocalPlayer();
        Console.WriteLine("");
        Console.WriteLine($"You: Health {you?.Health}   Location {you?.Place.ToString()}");
        Console.WriteLine("Choose a card to play");
        var deck = _getLocalPlayer()?.Deck;
        for (uint i = 0; i < deck?.Count; ++i)
        {
            Console.WriteLine($"[{i + 1}] {deck.Get(i).ToPrettyString()}");
        }
    }

    private void displayOpponent()
    {
        var opponent = _getRemotePlayer();
        Console.WriteLine("");
        Console.WriteLine($"Opponent: Health {opponent?.Health}   Location {opponent?.Place.ToString()}");
        var deck = _getRemotePlayer()?.Deck;
        for (uint i = 0; i < deck?.Count; ++i)
        {
            Console.WriteLine($"{deck.Get(i).ToPrettyString()}");
        }
    }

    public bool Closed { get; private set; } = false;


    private Match? match;
    public EPlayerType playerType;
    private Dictionary<ConsoleKey, bool> _burned = new Dictionary<ConsoleKey, bool>();

    public enum EPlayerType
    {
        Host,
        Challenger
    }
    public static NetGame? Host(PlayerDetails? localPlayer, PlayerDetails? remotePlayer)
    {
        return Play(localPlayer, remotePlayer, EPlayerType.Host);
    }

    public static NetGame? Challenge(PlayerDetails? localPlayer, PlayerDetails? remotePlayer)
    {
        return Play(remotePlayer, localPlayer, EPlayerType.Challenger);
    }
    private static NetGame? Play(PlayerDetails? hostDetails, PlayerDetails? challengerDetails, EPlayerType playerType)
    {
        var host = new PlayerController();
        if (hostDetails != null)
        {
            host.Seed = hostDetails.Seed;

            var challenger = new PlayerController();
            if (challengerDetails != null)
            {
                challenger.Seed = challengerDetails.Seed;

                var challenge = host.CreateChallenge();
                var match = challenger.AcceptChallenge(challenge);

                return new NetGame()
                {
                    HostName = hostDetails.Name, ChallengerName = challengerDetails.Name, match = match,
                    playerType = playerType
                };
            }
        }

        throw new InvalidProgramException();
    }

    public string? HostName { get; set; }
    
    public string? ChallengerName { get; set; }

    private NetGame()
    {
    }

    public void UpdateGame(Turn turn, Turn opponentsTurn)
    {
        Console.WriteLine();
        Console.WriteLine("You played {0}", displayTurn(turn, _getLocalPlayer()));
        Console.WriteLine("Opponent played {0}", displayTurn(opponentsTurn, _getRemotePlayer()));
        match?.SetVerbose(true);

        
        var me = _getLocalPlayer();
        if (turn.Message != null)
        {
            me?.PlayCard(turn.Message.Card, turn.Message.Data);

            var them = _getRemotePlayer();
            if (opponentsTurn.Message != null)
            {
                them?.PlayCard(opponentsTurn.Message.Card, opponentsTurn.Message.Data);

                match?.Resolve();
                if (_getLocalPlayer()!.Deck.Get(turn.Message.Card).Burned)
                {
                    _burned[ConsoleKey.D1 + (int)turn.Message.Card] = true;
                }
            }
        }

        if (match is { Complete: true })
        {
            Closed = true;
        }
    }

    private string displayTurn(Turn turn, PlayerController? player)
    {
        var sb = new StringBuilder();
        if (turn.Message != null)
        {
            var card = player?.Deck.Get(turn.Message.Card);
            sb.Append(card?.ToPrettyString());
            if (card != null)
                switch (card.Choice)
                {
                    case Choice.Move:
                        sb.Append($" move to {turn.Message.Data?[1]} to {turn.Message.Data?[0]}");
                        break;
                    case Choice.Strike:
                        sb.Append($" target is {turn.Message.Data?[0]}");
                        break;
                }
        }

        return sb.ToString();
    }

    private PlayerController? _getRemotePlayer()
    {
        return (playerType == EPlayerType.Host) ? match?.GetChallenger() : match?.GetHost();
    }

    private PlayerController? _getLocalPlayer()
    {
        return (playerType == EPlayerType.Host) ? match?.GetHost() : match?.GetChallenger();
    }

    public void Start()
    {
        match?.Start();
    }

    public string? Winner()
    {
        if (Closed)
        {
            if (match?.Winner == match?.GetHost())
            {
                return HostName;
            }

            if (match?.Winner == match?.GetChallenger())
            {
                return ChallengerName;
            }

            return "No one wins in a draw";
        }

        return "To be determined";
    }

    public Card? Reward() => match?.Basket.Reward;

    public void Tick(InGladioNetwork network)
    {
        Console.WriteLine($"{HostName} challenging {ChallengerName}");
        while (!Closed)
        {

            var turn = takeTurn();
            if (turn.Message != null)
            {
                network.SendMessage(turn.Message);

                Console.WriteLine("Waiting for opponent to play");
                network.WaitForMessages();

                var opponentsTurn = new Turn()
                {
                    Message = network.MessageQueue.Dequeue() as PlayMessage,
                    Local = false
                };

                UpdateGame(turn, opponentsTurn);
            }
        }

        Console.WriteLine("Game Over");
        Console.WriteLine("Winner {0}", Winner());
        Console.WriteLine("Reward is {0}", Reward()?.ToPrettyString());
    }
}