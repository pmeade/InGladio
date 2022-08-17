using lib;


namespace client;

public class NetGame
{
    enum EGameState
    {
        Init,
        Done
    }

    private EGameState gameState;
    private int round = 0;

    public Turn YourTurn()
    {
        dumpGameDetails();

        var key = ConsoleKey.Clear;
        while (key < ConsoleKey.D1 || key > ConsoleKey.D8)
        {
            Console.WriteLine("Getting Key");
            var input = Console.ReadKey();
            key = input.Key;
            Console.WriteLine("Got Key");
        }

        var cardIndex = (uint)(key - ConsoleKey.D1);
        var card = _getLocalPlayer().Deck.Get(cardIndex);
        string[]? data;
        switch (card.Choice)
        {
            case Choice.Move:
                var moveLocation = inputLocation();
                data = new string[] { moveLocation };
                return new Turn()
                {
                    Message = Message.Play(cardIndex, data),
                    Local = true
                };

            case Choice.Strike:
            case Choice.Parry:
            default:
                data = new string[] { };
                return new Turn()
                {
                    Message = Message.Play(cardIndex, data),
                    Local = true
                };
        }

    }

    private string inputLocation()
    {
        var key = TextInput.Get(new Dictionary<ConsoleKey, string>()
        {
            {
                ConsoleKey.S,
                "Square"
            },
            {
                ConsoleKey.P,
                "Perch"
            },
            {
                ConsoleKey.C,
                "Curtain"
            },
            {
                ConsoleKey.T,
                "Stairs"
            }
        });

        switch (key)
        {
            case ConsoleKey.S:
                return "Square";
            case ConsoleKey.P:
                return "Perch";
            case ConsoleKey.C:
                return "Curtain";
            default:
                return "Stairs";
        }
    }
    private void dumpBasket()
    {
        Console.WriteLine("Basket: {0}", match.Basket.Face.ToPrettyString());
    }

    private void dumpCards(Deck deck)
    {
        for (uint i = 0; i < deck.Count; ++i)
        {
            Console.WriteLine(String.Format("[{0}] {1}", i+1, deck.Get(i).ToPrettyString()));
        }
    }

    private void dumpScores()
    {
        Console.WriteLine("Host {0}     Challenger {1}     Basket {2}", 
            match.Challenge.Host.Health, match.Challenge.Challenger.Health, match.Basket.Health);
    }

    private void dumpGameDetails()
    {
        Console.WriteLine("In Gladio");
        Console.WriteLine("Round {0}", ++round);
        dumpScores();
        dumpCards(match.LeftDeck);
        dumpBasket();
        dumpCards(match.RightDeck);
    }

    public bool Closed { get; private set; } = false;


    private Match match;
    public EPlayerType playerType;

    public enum EPlayerType
    {
        Host,
        Challenger
    }
    public static NetGame Host(PlayerDetails localPlayer, PlayerDetails remotePlayer)
    {
        return Play(localPlayer, remotePlayer, EPlayerType.Host);
    }

    public static NetGame Challenge(PlayerDetails localPlayer, PlayerDetails remotePlayer)
    {
        return Play(remotePlayer, localPlayer, EPlayerType.Challenger);
    }
    private static NetGame Play(PlayerDetails hostDetails, PlayerDetails challengerDetails, EPlayerType playerType)
    {
        var host = new PlayerController();
        host.Seed = hostDetails.Seed;
        
        var challenger = new PlayerController();
        challenger.Seed = challengerDetails.Seed;

        var challenge = host.CreateChallenge();
        var match = challenger.AcceptChallenge(challenge);
        
        return new NetGame() { HostName = hostDetails.Name, ChallengerName = challengerDetails.Name, match = match, playerType = playerType};
    }

    public string HostName { get; set; }
    
    public string ChallengerName { get; set; }

    private NetGame()
    {
        gameState = EGameState.Init;
    }

    public void UpdateGame(Turn turn, Turn opponentsTurn)
    {
        var me = _getLocalPlayer();
        me.PlayCard(turn.Message.Card, turn.Message.Data);

        var them = _getRemotePlayer();
        them.PlayCard(opponentsTurn.Message.Card, opponentsTurn.Message.Data);
        
        match.Resolve(true);
        if (match.Complete)
        {
            Closed = true;
        }
    }

    private PlayerController _getRemotePlayer()
    {
        return (playerType == EPlayerType.Host) ? match.GetChallenger() : match.GetHost();
    }

    private PlayerController _getLocalPlayer()
    {
        return (playerType == EPlayerType.Host) ? match.GetHost() : match.GetChallenger();
    }

    public void Start()
    {
        match.Start();
    }

    public string Winner()
    {
        if (Closed)
        {
            if (match.Winner == match.GetHost())
            {
                return HostName;
            }

            if (match.Winner == match.GetChallenger())
            {
                return ChallengerName;
            }

            return "No one wins in a draw";
        }

        return "To be determined";
    }

    public Card Reward() => match.Basket.Reward;
}