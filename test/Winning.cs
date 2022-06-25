using lib;

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
        this.match = leftPlayer.StartMatch();
    }

    private Card rock()
    {
        return new Card();
    }

    [Test]
    public void WinOnOpponentWithZeroHealth()
    {
        Assert.IsFalse(leftPlayer.Dead);
        Assert.IsTrue(match.Active);
        Assert.IsTrue(match.Started);

        leftPlayer.ChooseMove(rock(), Place.Square, leftPlayer);
        rightPlayer.ChooseStrike(rock(), leftPlayer);
        match.Resolve();
        leftPlayer.ChooseMove(rock(), Place.Square, leftPlayer);
        rightPlayer.ChooseStrike(rock(), leftPlayer);
        match.Resolve();
        leftPlayer.ChooseMove(rock(), Place.Square, leftPlayer);
        rightPlayer.ChooseStrike(rock(), leftPlayer);
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

        leftPlayer.ChooseStrike(rock(), rightPlayer);
        rightPlayer.ChooseStrike(rock(), leftPlayer);
        match.Resolve();
        leftPlayer.ChooseStrike(rock(), rightPlayer);
        rightPlayer.ChooseStrike(rock(), leftPlayer);
        match.Resolve();
        leftPlayer.ChooseStrike(rock(), rightPlayer);
        rightPlayer.ChooseStrike(rock(), leftPlayer);
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

        leftPlayer.ChooseMove(rock(), Place.Square, leftPlayer);
        rightPlayer.ChooseStrike(rock(), match.Basket);
        match.Resolve();
        leftPlayer.ChooseMove(rock(), Place.Square, leftPlayer);
        rightPlayer.ChooseStrike(rock(), match.Basket);
        match.Resolve();
        leftPlayer.ChooseMove(rock(), Place.Square, leftPlayer);
        rightPlayer.ChooseStrike(rock(), match.Basket);
        match.Resolve();

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

        leftPlayer.ChooseMove(rock(), Place.Square, leftPlayer);
        rightPlayer.ChooseMove(rock(), Place.Square, rightPlayer);
        match.Resolve();
        leftPlayer.ChooseStrike(rock(), match.Basket);
        rightPlayer.ChooseStrike(rock(), match.Basket);
        match.Resolve();
        leftPlayer.ChooseStrike(rock(), match.Basket);
        rightPlayer.ChooseStrike(rock(), match.Basket);
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

        leftPlayer.ChooseMove(rock(), Place.Square, leftPlayer);
        rightPlayer.ChooseMove(rock(), Place.Square, rightPlayer);
        match.Resolve();
        leftPlayer.ChooseStrike(rock(), match.Basket);
        rightPlayer.ChooseStrike(rock(), match.Basket);
        match.Resolve();
        leftPlayer.ChooseStrike(rock(), match.Basket);
        rightPlayer.ChooseStrike(rock(), match.Basket);
        match.Resolve();
        leftPlayer.ChooseStrike(rock(), match.Basket);
        rightPlayer.ChooseMove(rock(), Place.Curtain, rightPlayer);
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
            leftPlayer.ChooseMove(rock(), Place.Steps, leftPlayer);
            rightPlayer.ChooseMove(rock(), Place.Steps, rightPlayer);
            match.Resolve();
        }
        
        Assert.False(match.Active);
    }
}