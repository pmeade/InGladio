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
        Assert.IsNull(match.Basket.Reward);
    }
}