using lib;


namespace client;

public class Game
{
    enum EGameState
    {
        Init,
        Done
    }

    private EGameState gameState;
    
    public void Handle(Message message)
    {
        Console.WriteLine("Handling {0}", message.MessageType.ToString());
        if (message.MessageType == Message.EMessageType.Quit)
        {
            Console.WriteLine("Closing Game");
            Closed = true;
        }
    }

    public List<Message> YourTurn()
    {
        dumpGameDetails();
        var messages = new List<Message>();
        switch (gameState)
        {
            case EGameState.Init:
                Console.WriteLine("Game Ready");
                dumpScores();
                dumpCards(game.Host);
                dumpBasket();
                dumpCards(game.Challenger);
                // messages.Add(makePlay());
                break;
                
        }
        return new List<Message>(){Message.Done()};
    }

    // private Message makePlay()
    // {
    //
    //     bool cardPicked = false;
    //     Card pickedCard;
    //     while (!cardPicked)
    //     {
    //         Console.WriteLine("Choose card: 1-8");
    //         var key = Console.ReadKey();
    //         if (key.Key >= ConsoleKey.D1 && key.Key <= ConsoleKey.D8)
    //         {
    //             pickedCard = game.Match.LeftDeck.Get(uint.Parse(key.Key.ToString()));
    //             cardPicked = true;
    //         }
    //     }
    //
    //     bool choiceMade = false;
    //     Play play;
    //     while (!choiceMade)
    //     {
    //         Console.WriteLine("[M]ove [S]trike [P]arry");
    //         var key = Console.ReadKey();
    //         switch (key.Key)
    //         {
    //             case ConsoleKey.M:
    //                 choiceMade = true;
    //                 break;
    //             case ConsoleKey.S:
    //                 choiceMade = true;
    //                 break;
    //             case ConsoleKey.P:
    //                 choiceMade = true;
    //                 break;
    //         }
    //     }
    // }

    // private Play makeMove(Card card)
    // {
    //     Console.WriteLine("[S]teps S[q]uare [P]erch [C]urtain");
    //     Console.ReadKey();
    // }

    private void dumpBasket()
    {
        Console.WriteLine("Basket: {0}", game.Match.Basket.Card.ToString());
    }

    private void dumpCards(PlayerController player)
    {
        var deck = player == game.Host ? game.Match.LeftDeck : game.Match.RightDeck;
        Console.WriteLine("{0} {1} {2}", 
            deck.Get(0).ToString(),
            deck.Get(1).ToString(),
            deck.Get(2).ToString()
        );
        Console.WriteLine("{0} {1} {2}", 
            deck.Get(3).ToString(),
            deck.Get(4).ToString(),
            deck.Get(5).ToString()
        );
        Console.WriteLine("{0} {1} ", 
            deck.Get(6).ToString(),
            deck.Get(7).ToString()
        );
    }

    private void dumpScores()
    {
        Console.WriteLine("Host {0}     Challenger {1}     Basket {2}", 
            game.Host.Health, game.Challenger.Health, game.Basket.Health);
    }

    private void dumpGameDetails()
    {
        Console.WriteLine("In Gladio");
    }

    public bool Closed { get; private set; } = false;


    private lib.Game game;
    // public void Start()
    // {
    //     game = lib.Game.Create();
    // }
}