using lib;

namespace test;

public class Baskets
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
    public void BasketHidesContentsUntilOpen()
    {
        Assert.IsFalse(match.Basket.Open);
        Assert.IsNull(match.Basket.Card);
    }

    [Test]
    public void BasketCreatesACardWith17RollsAtLootTable()
    {
        Assert.IsNull(match.Basket.Card);
        leftPlayer.ChooseStrike(new Card(), rightPlayer);
        rightPlayer.ChooseParry(new Card());
        match.Resolve();
        leftPlayer.ChooseStrike(new Card(), rightPlayer);
        rightPlayer.ChooseParry(new Card());
        match.Resolve();
        Assert.IsNotNull(match.Basket.Card);
        Assert.IsTrue(match.Basket.Card.CardAttributes.Count == 17);
    }
}