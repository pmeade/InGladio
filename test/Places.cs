using lib;

namespace test;

public class Places
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

    class TestTarget : Target
    {
        internal int Health = 3;

        private TestTarget(Place location, Target targetOfTarget)
        {
            Location = location;
            ActivePlay = Play.Strike(Card.BasicStrike(), targetOfTarget, location);
        }
        
        public TestTarget(Place location)
        {
            var targetOfTarget = new TestTarget(location, this);
            Location = location;
            ActivePlay = Play.Strike(Card.BasicStrike(), targetOfTarget, location);
        }

        public void TakeDamage(int amount, PlayerController dealer)
        {
            Health -= amount;
        }

        public void UpdateLocation(Place place) => Location = place;

        public Place Location { get; private set; }
        public Play ActivePlay { get; }
    }
    
    [Test]
    public void StepsAreMinusOnePowerToAttack()
    {
        var target = new TestTarget(Place.Square);
        {
            var card = new Card() { Power = Power.Three, Choice = Choice.Strike, Platonic = Platonic.Rock };
            var play = new Play(card, target, Place.Steps);
            Assert.IsTrue(play.EffectivePower == Power.Three);
        }
        {
            var card = new Card() { Power = Power.Five, Choice = Choice.Strike, Platonic = Platonic.Rock };
            var play = new Play(card, target, Place.Steps);
            Assert.IsTrue(play.EffectivePower == Power.Three);
        }
        {
            var card = new Card() { Power = Power.Eight, Choice = Choice.Strike, Platonic = Platonic.Rock };
            var play = new Play(card, target, Place.Steps);
            Assert.IsTrue(play.EffectivePower == Power.Five);
        }
    }
    
    [Test]
    public void PerchIsPlusOneToAttack()
    {
        var target = new TestTarget(Place.Square);
        {
            var card = new Card() { Power = Power.Three, Choice = Choice.Strike, Platonic = Platonic.Rock };
            var play = new Play(card, target, Place.Perch);
            Assert.IsTrue(play.EffectivePower == Power.Five);
        }
        {
            var card = new Card() { Power = Power.Five, Choice = Choice.Strike, Platonic = Platonic.Rock };
            var play = new Play(card, target, Place.Perch);
            Assert.IsTrue(play.EffectivePower == Power.Eight);
        }
        {
            var card = new Card() { Power = Power.Eight, Choice = Choice.Strike, Platonic = Platonic.Rock };
            var play = new Play(card, target, Place.Perch);
            Assert.IsTrue(play.EffectivePower == Power.Eight);
        }
    }

    [Test]
    public void CurtainIsMinusOneToAttackInOrOut()
    {
        var target = new TestTarget(Place.Square);
        {
            var card = new Card() { Power = Power.Three, Choice = Choice.Strike, Platonic = Platonic.Rock };
            var play = new Play(card, target, Place.Curtain);
            Assert.IsTrue(play.EffectivePower == Power.Three);
        }
        {
            var card = new Card() { Power = Power.Five, Choice = Choice.Strike, Platonic = Platonic.Rock };
            var play = new Play(card, target, Place.Curtain);
            Assert.IsTrue(play.EffectivePower == Power.Three);
        }
        {
            var card = new Card() { Power = Power.Eight, Choice = Choice.Strike, Platonic = Platonic.Rock };
            var play = new Play(card, target, Place.Curtain);
            Assert.IsTrue(play.EffectivePower == Power.Five);
        }
        
        target = new TestTarget(Place.Curtain);
        {
            var card = new Card() { Power = Power.Three, Choice = Choice.Strike, Platonic = Platonic.Rock };
            var play = new Play(card, target, Place.Square);
            Assert.IsTrue(play.EffectivePower == Power.Three);
        }
        {
            var card = new Card() { Power = Power.Five, Choice = Choice.Strike, Platonic = Platonic.Rock };
            var play = new Play(card, target, Place.Square);
            Assert.IsTrue(play.EffectivePower == Power.Three);
        }
        {
            var card = new Card() { Power = Power.Eight, Choice = Choice.Strike, Platonic = Platonic.Rock };
            var play = new Play(card, target, Place.Square);
            Assert.IsTrue(play.EffectivePower == Power.Five);
        }
    }
}