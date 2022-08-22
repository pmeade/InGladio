using lib;

namespace test;

public class Cards
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
        this.match = leftPlayer.StartMatch(true);
    }

    [Test]
    public void FaceIsRockPaperOrScissors()
    {
        var rock = new Card() { Platonic = Platonic.Rock };
        var paper = new Card { Platonic = Platonic.Paper };
        var scissors = new Card { Platonic = Platonic.Scissors };
        
        Assert.IsTrue(rock.Platonic == Platonic.Rock);
        Assert.IsTrue(paper.Platonic == Platonic.Paper);
        Assert.IsTrue(scissors.Platonic == Platonic.Scissors);
    }

    [Test]
    public void PowerIsThreeFiveOrEight()
    {
        var three = new Card() { Power = Power.Three };
        var five = new Card() { Power = Power.Five };
        var eight = new Card() { Power = Power.Eight };
        
        Assert.IsTrue(three.Power == Power.Three);
        Assert.IsTrue(five.Power == Power.Five);
        Assert.IsTrue(eight.Power == Power.Eight);
    }

    [Test]
    public void ActionOnWinHand()
    {
        var healSelfWhenWinHandCard = new Card()
        {
            Choice = Choice.Move, Platonic = Platonic.Paper, Power = Power.Three, OnWinHand = controller => controller.HealSelf(3)
        };
        
        Assert.IsTrue(match.Active);
        Assert.IsTrue(leftPlayer.Health == 3);

        leftPlayer.ChooseMove(Card.BasicMove(), Place.Square, leftPlayer, Place.Square);
        rightPlayer.ChooseStrike(Card.BasicStrike(), leftPlayer, Place.Square);
        match.Resolve();
        Assert.IsTrue(leftPlayer.Health == 2);
        
        // heal
        leftPlayer.ChooseMove(healSelfWhenWinHandCard, Place.Square, leftPlayer, Place.Square);
        rightPlayer.ChooseParry(Card.BasicParry(), Place.Square);
        match.Resolve();
        Assert.IsTrue(leftPlayer.Health == 3);
    }
    
    [Test]
    public void ActionOnBurned()
    {
        var healSelfWhenBurnedCard = new Card()
        {
            Platonic = Platonic.Paper, Power = Power.Three, OnBurned = controller => controller.HealSelf(3)
        };
        
        Assert.IsTrue(match.Active);
        Assert.IsTrue(leftPlayer.Health == 3);

        leftPlayer.ChooseMove(new Card(){Choice = Choice.Move}, Place.Square, leftPlayer, Place.Square);
        rightPlayer.ChooseStrike(new Card(){Choice = Choice.Strike}, leftPlayer, Place.Square);
        match.Resolve();
        Assert.IsTrue(leftPlayer.Health == 2);
        
        // heal
        Assert.IsTrue(healSelfWhenBurnedCard.Platonic == Platonic.Paper);
        leftPlayer.ChooseMove(healSelfWhenBurnedCard, Place.Square, leftPlayer, Place.Square);
        rightPlayer.ChooseMove(new Card(){Platonic = Platonic.Scissors}, Place.Square, rightPlayer, Place.Square);
        match.Resolve();
        Assert.IsTrue(leftPlayer.Health == 3);
    }

    [Test]
    public void NeedsToMatchPowerOrBetterToWinHand()
    {
        Assert.IsTrue(match.Active);
        Assert.IsTrue(leftPlayer.Health == 3);

        leftPlayer.ChooseMove(new Card(){Choice = Choice.Move, Power = Power.Five}, Place.Square, leftPlayer, Place.Square);
        rightPlayer.ChooseStrike(new Card(){Choice = Choice.Strike, Power = Power.Three}, leftPlayer, Place.Square);
        match.Resolve();
        Assert.IsTrue(leftPlayer.Health == 3);

        leftPlayer.ChooseMove(new Card(){Choice = Choice.Move, Power = Power.Five}, Place.Square, leftPlayer, Place.Square);
        rightPlayer.ChooseStrike(new Card(){Choice = Choice.Strike, Power = Power.Five}, leftPlayer, Place.Square);
        match.Resolve();
        Assert.IsTrue(leftPlayer.Health == 2);
    }
}