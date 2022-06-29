using lib;

namespace test;

public class Client
{
    private ClientController clientController;

    [SetUp]
    public void SetUp()
    {
        this.clientController = new ClientController();
    }

    [Test]
    public void YouCanAddCardsToYourLibrary()
    {
        Card[] testCards = new[] { 
            new Card() { Platonic = Platonic.Rock },
            new Card() {Platonic = Platonic.Paper},
            new Card() {Platonic = Platonic.Scissors}};
        clientController.AddCards(testCards);
        Assert.That(clientController.Cards, Is.EqualTo(testCards));
    }

    [Test]
    public void YouCanBuildADeckWithEightOfYourCards()
    {
        Card[] testCards = new[] { 
            new Card() { Platonic = Platonic.Rock, Power = Power.Three},
            new Card() {Platonic = Platonic.Paper, Power = Power.Three},
            new Card() {Platonic = Platonic.Scissors, Power = Power.Three},
            new Card() { Platonic = Platonic.Rock, Power = Power.Five },
            new Card() {Platonic = Platonic.Paper, Power = Power.Five},
            new Card() {Platonic = Platonic.Scissors, Power = Power.Five},
            new Card() { Platonic = Platonic.Rock, Power = Power.Eight},
            new Card() {Platonic = Platonic.Paper, Power = Power.Eight}
        };
        
        clientController.AddCards(testCards);

        Assert.That(clientController.Decks.Count, Is.EqualTo(0));
        var deck = clientController.MakeDeck(new int[]{0,1,2,3,4,5,6,7});
        Assert.That(clientController.Decks.Count, Is.EqualTo(1));
    }

    [Test]
    public void YouCanCreateAChallengeWithOneOfYourDecks()
    {
        Card[] testCards = new[] { 
            new Card() { Platonic = Platonic.Rock, Power = Power.Three},
            new Card() {Platonic = Platonic.Paper, Power = Power.Three},
            new Card() {Platonic = Platonic.Scissors, Power = Power.Three},
            new Card() { Platonic = Platonic.Rock, Power = Power.Five },
            new Card() {Platonic = Platonic.Paper, Power = Power.Five},
            new Card() {Platonic = Platonic.Scissors, Power = Power.Five},
            new Card() { Platonic = Platonic.Rock, Power = Power.Eight},
            new Card() {Platonic = Platonic.Paper, Power = Power.Eight}
        };
        
        clientController.AddCards(testCards);
        clientController.MakeDeck(new int[]{0,1,2,3,4,5,6,7});
        var challenge = clientController.MakeChallenge(0);
        Assert.That(challenge.Open == true);
        Assert.That(challenge.Deck.Unsealed(), Is.EqualTo(clientController.Decks[0]));
    }

    [Test]
    public void YouCanCreateAChallengeWithARandomDeck()
    {
        var challenge = clientController.MakeChallenge();
        Assert.That(challenge.Open == true);
        Assert.That(challenge.Deck.Unsealed().Count, Is.EqualTo(8));
    }

    [Test]
    public void YouGetNotifiedWhenSomeoneAcceptsYourChallenge()
    {
        var accepted = false;
        clientController.ChallengeAccepted += (sender, args) => accepted = true; 
        var challenge = clientController.MakeChallenge();
        PlayerController someoneElse = new PlayerController();
        
        Assert.That(accepted, Is.EqualTo(false));
        someoneElse.AcceptChallenge(challenge);
        Assert.That(accepted, Is.EqualTo(true));
    }
}