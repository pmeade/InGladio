using lib;

namespace test;

public class Decks
{
    private Match match;
    private PlayerController leftPlayer;
    private PlayerController rightPlayer;
    
    [SetUp]
    public void Setup()
    {
        this.leftPlayer = new PlayerController();
        this.rightPlayer = new PlayerController();
        var challenge = leftPlayer.CreateChallenge();
        rightPlayer.AcceptChallenge(challenge);
        this.match = leftPlayer.StartMatch();
    }
    
    [Test]
    public void RandomDeckHasEightCards()
    {
        var deck = Deck.Random();
        Assert.IsTrue(deck.Count == 8);
    }

    [Test]
    public void PrebuiltDeckFillsEmptySpots()
    {
        var card1 = new Card() { Platonic = Platonic.Paper, Power = Power.Eight };
        var deck = Deck.FromCards(new Card[] { card1 });
        Assert.IsTrue(deck.Count == 8);
        Assert.IsTrue(deck.Get(0) == card1);
    }

    [Test]
    public void CanPlayCardFromDeck()
    {
        leftPlayer.ChooseStrike(match.LeftDeck.Play(0), rightPlayer);
    }
}