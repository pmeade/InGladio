using lib;
using lib.CardExtensionMethods;

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

        leftPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        rightPlayer.StrikeOpponent(Card.BasicStrike());
        match.Resolve();
        Assert.IsTrue(leftPlayer.Health == 2);
        
        // heal
        leftPlayer.MoveSelf(healSelfWhenWinHandCard, Place.Square);
        rightPlayer.ParryOpponent(Card.BasicParry());
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

        leftPlayer.MoveSelf(new Card(){Choice = Choice.Move}, Place.Square);
        rightPlayer.StrikeOpponent(new Card(){Choice = Choice.Strike});
        match.Resolve();
        Assert.IsTrue(leftPlayer.Health == 2);
        
        // heal
        Assert.IsTrue(healSelfWhenBurnedCard.Platonic == Platonic.Paper);
        leftPlayer.MoveSelf(healSelfWhenBurnedCard, Place.Square);
        rightPlayer.MoveSelf(new Card(){Platonic = Platonic.Scissors}, Place.Square);
        match.Resolve();
        Assert.IsTrue(leftPlayer.Health == 3);
    }

    [Test]
    public void NeedsToMatchPowerOrBetterToWinHand()
    {
        Assert.IsTrue(match.Active);
        Assert.IsTrue(leftPlayer.Health == 3);

        leftPlayer.UpdateLocation(Place.Square);
        rightPlayer.UpdateLocation(Place.Square);

        leftPlayer.StrikeOpponent(Card.BasicStrike().AsThree());
        rightPlayer.StrikeOpponent(Card.BasicStrike().AsThree());
        match.Resolve();
        Assert.IsTrue(leftPlayer.Health == 2);
        Assert.IsTrue(rightPlayer.Health == 2);

        leftPlayer.StrikeOpponent(Card.BasicStrike().AsFive());
        rightPlayer.StrikeOpponent(Card.BasicStrike().AsThree());
        match.Resolve();
        Assert.IsTrue(leftPlayer.Health == 2);
        Assert.IsTrue(rightPlayer.Health == 1);
    }
}