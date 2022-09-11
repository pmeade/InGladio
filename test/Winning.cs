using lib;
using lib.CardExtensionMethods;

namespace test;

public class Winning
{
    private PlayerController leftPlayer;
    private PlayerController rightPlayer;
    private Match match;

    [SetUp]
    public void Setup()
    {
        this.leftPlayer = new PlayerController();
        this.rightPlayer = new PlayerController();
        var challenge = leftPlayer.CreateChallenge();
        rightPlayer.AcceptChallenge(challenge);
        this.match = leftPlayer.StartMatch(true);
    }

    [Test]
    public void WinOnOpponentWithZeroHealth()
    {
        Assert.IsFalse(leftPlayer.Dead);
        Assert.IsTrue(match.Active);
        Assert.IsTrue(match.Started);

        leftPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        rightPlayer.StrikeOpponent(Card.BasicStrike());
        match.Resolve();
        leftPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        rightPlayer.StrikeOpponent(Card.BasicStrike());
        match.Resolve();
        leftPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        rightPlayer.StrikeOpponent(Card.BasicStrike());
        match.Resolve();

        Assert.IsTrue(leftPlayer.Dead);
        Assert.IsFalse(match.Active);
        Assert.IsTrue(match.Complete);
        Assert.IsTrue(match.Winner == rightPlayer);
    }

    [Test]
    public void DrawOnBothPlayersWithZeroHealth()
    {
        Assert.IsFalse(leftPlayer.Dead);
        Assert.IsFalse(rightPlayer.Dead);
        Assert.IsTrue(match.Active);
        Assert.IsTrue(match.Started);

        leftPlayer.StrikeOpponent(Card.BasicStrike());
        rightPlayer.StrikeOpponent(Card.BasicStrike());
        match.Resolve();
        leftPlayer.StrikeOpponent(Card.BasicStrike());
        rightPlayer.StrikeOpponent(Card.BasicStrike());
        match.Resolve();
        leftPlayer.StrikeOpponent(Card.BasicStrike());
        rightPlayer.StrikeOpponent(Card.BasicStrike());
        match.Resolve();

        Assert.IsTrue(leftPlayer.Dead);
        Assert.IsTrue(rightPlayer.Dead);
        Assert.IsFalse(match.Active);
        Assert.IsTrue(match.Complete);
        Assert.IsTrue(match.Winner == null);
    }

    [Test]
    public void WinWhenStrikingBasketToZero()
    {
        Assert.IsFalse(match.Basket.Open);
        Assert.IsTrue(match.Active);
        Assert.IsTrue(match.Started);

        int basketHealth = 3;
        while (basketHealth > 0)
        {
            leftPlayer.MoveSelf(Card.BasicMove(), Place.Square);
            if (rightPlayer.Location == Place.Steps)
            {
                rightPlayer.MoveSelf(Card.BasicMove(), Place.Square);
            }
            else if (match.Basket.Face.Choice == Choice.Move)
            {
                rightPlayer.StrikeBasket(Card.BasicStrike().AsEight());
                basketHealth -= 1;
            }
            else if (match.Basket.Face.Choice == Choice.Strike)
            {
                rightPlayer.ParryBasket(Card.BasicParry().AsEight());
                basketHealth -= 2;
            }
            else
            {
                rightPlayer.MoveSelf(Card.BasicMove(), Place.Square);
            }

            match.Resolve();
        }

        Assert.IsTrue(match.Basket.Health == 0);
        Assert.IsTrue(match.Basket.Open);
        Assert.IsFalse(leftPlayer.Dead);
        Assert.IsFalse(match.Active);
        Assert.IsTrue(match.Complete);
        Assert.IsTrue(match.Winner == rightPlayer);
    }

    [Test]
    public void GameContinuesWhenBothStrikeBasketToZero()
    {
        Assert.IsFalse(match.Basket.Open);
        Assert.IsTrue(match.Active);
        Assert.IsTrue(match.Started);

        leftPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        rightPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        match.Resolve();
        leftPlayer.StrikeOpponent(Card.BasicStrike());
        rightPlayer.StrikeOpponent(Card.BasicStrike());
        match.Resolve();
        leftPlayer.StrikeOpponent(Card.BasicStrike());
        rightPlayer.StrikeOpponent(Card.BasicStrike());
        match.Resolve();

        Assert.IsFalse(match.Basket.Open);
        Assert.IsTrue(match.Active);
        Assert.IsFalse(match.Complete);
        Assert.IsTrue(match.Winner == null);
    }

    [Test]
    public void WinWhenStrikingBasketAlreadyAtZero()
    {
        Assert.IsFalse(match.Basket.Open);
        Assert.IsTrue(match.Active);
        Assert.IsTrue(match.Started);

        leftPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        rightPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        match.Resolve();
        leftPlayer.StrikeOpponent(Card.BasicStrike());
        rightPlayer.StrikeOpponent(Card.BasicStrike());
        match.Resolve();
        leftPlayer.StrikeOpponent(Card.BasicStrike());
        rightPlayer.StrikeOpponent(Card.BasicStrike());
        match.Resolve();
        leftPlayer.StrikeOpponent(Card.BasicStrike());
        rightPlayer.MoveSelf(Card.BasicMove(), Place.Curtain);
        match.Resolve();

        Assert.IsTrue(match.Basket.Open);
        Assert.IsFalse(match.Active);
        Assert.IsTrue(match.Complete);
        Assert.IsTrue(match.Winner == leftPlayer);
    }

    [Test]
    public void DrawWhenEightTurnsExpireWithoutVictory()
    {
        for (int i = 0; i < 8; i++)
        {
            Assert.IsTrue(match.Active);
            Assert.IsTrue(match.Round == i);
            leftPlayer.MoveSelf(Card.BasicMove(), Place.Steps);
            rightPlayer.MoveSelf(Card.BasicMove(), Place.Steps);
            match.Resolve();
        }
        
        Assert.False(match.Active);
    }
}