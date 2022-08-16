using lib;

namespace test;

public class Starting
{
    private PlayerController leftPlayer;
    private PlayerController rightPlayer;
    
    [SetUp]
    public void Setup()
    {
        leftPlayer = new PlayerController();
        rightPlayer = new PlayerController();
    }

    [Test]
    public void PlayerCanCreateChallenge()
    {
        var challenge = leftPlayer.CreateChallenge();
        Assert.True(challenge.Open);
        Assert.True(challenge.Host == leftPlayer);
    }

    [Test]
    public void PlayerMayOnlyHaveOneOpenChallenge()
    {
        var challenge = leftPlayer.CreateChallenge();
        var challenge2 = leftPlayer.CreateChallenge();
        Assert.False(challenge2.Open);
    }

    [Test]
    public void OtherMayAcceptAnOpenChallenge()
    {
        var challenge = leftPlayer.CreateChallenge();
        var acceptedMatch = rightPlayer.AcceptChallenge(challenge);

        Assert.False(challenge.Open);
        Assert.True(challenge.challenger == rightPlayer);
        Assert.True(leftPlayer.Opponent == rightPlayer);
        Assert.True(rightPlayer.Opponent == leftPlayer);
        Assert.True(acceptedMatch.Challenge == challenge);
        Assert.True(acceptedMatch.Active == true);
        Assert.True(acceptedMatch.Started == false);
    }

    private Match createAcceptedMatch()
    {
        var challenge = leftPlayer.CreateChallenge();
        var acceptedMatch = rightPlayer.AcceptChallenge(challenge);
        return acceptedMatch;
    }

    [Test]
    public void OffererMayStartAcceptedMatch()
    {
        var match = createAcceptedMatch();
        Assert.True(match.Started == false);
        leftPlayer.StartMatch();
        Assert.True(match.Started == true);
    }

    [Test]
    public void PlayersCanChallengeWithADeck()
    {
        var leftDeck = Deck.Random(Generator.FromSeed(1));
        var rightDeck = Deck.Random(Generator.FromSeed(1));

        Assert.False(leftDeck.Equals(rightDeck));
        
        var challenge = leftPlayer.CreateChallenge(leftDeck.Sealed());
        var match = rightPlayer.AcceptChallenge(challenge, rightDeck);
        
        Assert.True(match.LeftDeck.Equals(leftDeck));
        Assert.True(match.RightDeck.Equals(rightDeck));
        Assert.True(match.Active);
    }

    [Test]
    public void PlayersCanChallengeWithoutADeck()
    {
        var challenge = leftPlayer.CreateChallenge();
        var match = rightPlayer.AcceptChallenge(challenge);
        Assert.False(match.LeftDeck.Equals(match.RightDeck));
        Assert.True(match.Active);
    }

    [Test]
    public void PremadeDecksAreSealedUntilActive()
    {
        var leftDeck = Deck.Random(Generator.FromSeed(1));
        Assert.NotNull(leftDeck.Get(0));
        var sealedDeck = leftDeck.Sealed();
        var challenge = leftPlayer.CreateChallenge(sealedDeck);
        var match = rightPlayer.AcceptChallenge(challenge, Deck.Random(Generator.FromSeed(1)));
        
        Assert.IsTrue(match.LeftDeck.Equals(leftDeck));
    }
}

