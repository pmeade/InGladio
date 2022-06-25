using lib;

namespace test;

public class Cards
{
    [SetUp]
    public void Setup()
    {
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
}