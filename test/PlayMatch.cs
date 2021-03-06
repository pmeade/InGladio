using lib;

namespace test;

public class Playing
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
    public void PlayerCanPlayCardFromDeck()
    {
        Assert.True(leftPlayer.CanPlay());
        var deck = Deck.Random();
        leftPlayer.ChooseMove(deck.Get(0), Place.Square, leftPlayer);
        Assert.False(leftPlayer.CanPlay());
    }

    [Test]
    public void RockBurnsScissors()
    {
        var leftCard = new Card();
        leftPlayer.ChooseMove(leftCard, lib.Place.Square, leftPlayer);
        rightPlayer.ChooseMove(new Card(){Platonic = Platonic.Scissors}, Place.Square, rightPlayer);
        match.Resolve();
        var board = match.Board;
        Assert.True(board.LeftCard == leftCard);
        Assert.True(board.RightCard == null);
    }
    
    [Test]
    public void ScissorsBurnsPaper()
    {
        var leftCard = new Card(){Platonic = Platonic.Scissors};
        leftPlayer.ChooseMove(leftCard, lib.Place.Square, leftPlayer);
        rightPlayer.ChooseMove(new Card(){Platonic = Platonic.Paper}, Place.Square, rightPlayer);
        match.Resolve();
        var board = match.Board;
        Assert.True(board.LeftCard == leftCard);
        Assert.True(board.RightCard == null);
    }
    
    [Test]
    public void PaperBurnsRock()
    {
        var leftCard = new Card(){Platonic = Platonic.Paper};
        leftPlayer.ChooseMove(leftCard, lib.Place.Square, leftPlayer);
        rightPlayer.ChooseMove(new Card(), Place.Square, rightPlayer);
        var board = match.Board;
        match.Resolve();
        Assert.True(board.LeftCard == leftCard);
        Assert.True(board.RightCard == null);
    }

    [Test]
    public void MoveSucceedsVsMove()
    {
        var board = match.Board;
        Assert.True(leftPlayer.Place == Place.Steps);
        leftPlayer.ChooseMove(new Card(), Place.Square, leftPlayer);
        rightPlayer.ChooseMove(new Card(), Place.Square, rightPlayer);
        match.Resolve();
        Assert.True(leftPlayer.Place == Place.Square);
    }

    [Test]
    public void MoveSucceedsVsParry()
    {
        var board = match.Board;
        Assert.True(leftPlayer.Place == Place.Steps);
        leftPlayer.ChooseMove(new Card(), Place.Square, leftPlayer);
        rightPlayer.ChooseParry(new Card());
        match.Resolve();
        Assert.True(leftPlayer.Place == Place.Square);
    }

    [Test]
    public void MoveFailsVsStrike()
    {
        var board = match.Board;
        Assert.True(leftPlayer.Place == Place.Steps);
        leftPlayer.ChooseMove(new Card(), Place.Square, rightPlayer);
        rightPlayer.ChooseStrike(new Card(), leftPlayer);
        match.Resolve();
        Assert.True(leftPlayer.Place == Place.Steps);
    }
    
    [Test]
    public void StrikeSucceedsVsStrike()
    {
        Assert.True(rightPlayer.Health == 3);
        leftPlayer.ChooseStrike(new Card(), rightPlayer);
        rightPlayer.ChooseStrike(new Card(), leftPlayer);
        match.Resolve();
        Assert.True(rightPlayer.Health == 2);
    }
    
    [Test]
    public void StrikeSucceedsVsMove()
    {
        Assert.True(rightPlayer.Health == 3);
        leftPlayer.ChooseStrike(new Card(), rightPlayer);
        rightPlayer.ChooseMove(new Card(), Place.Square, rightPlayer);
        match.Resolve();
        Assert.True(rightPlayer.Health == 2);
    }
    
    [Test]
    public void StrikeFailsVsParry()
    {
        Assert.True(rightPlayer.Health == 3);
        leftPlayer.ChooseStrike(new Card(), rightPlayer);
        rightPlayer.ChooseParry(new Card());
        match.Resolve();
        Assert.True(rightPlayer.Health == 3);
    }

    [Test]
    public void ParrySucceedsVsStrike()
    {
        Assert.True(rightPlayer.Health == 3);
        leftPlayer.ChooseParry(new Card());
        rightPlayer.ChooseStrike(new Card(), leftPlayer);
        match.Resolve();
        Assert.True(rightPlayer.Health == 1);
    }

    [Test]
    public void ParryFailsVsMove()
    {
        Assert.True(rightPlayer.Health == 3);
        rightPlayer.ChooseParry(new Card());
        rightPlayer.ChooseMove(new Card(), Place.Square, leftPlayer);
        match.Resolve();
        Assert.True(rightPlayer.Health == 3);
    }

    [Test]
    public void ParryFailsVsParry()
    {
        Assert.True(rightPlayer.Health == 3);
        rightPlayer.ChooseParry(new Card());
        rightPlayer.ChooseParry(new Card());
        match.Resolve();
        Assert.True(rightPlayer.Health == 3);
    }

    [Test]
    public void PlayerStartsAt3Health()
    {
        Assert.True(leftPlayer.Health == 3);
        Assert.True(rightPlayer.Health == 3);
    }

    [Test]
    public void PlayerDiesAt0Health()
    {
        leftPlayer.ChooseStrike(new Card(), rightPlayer);
        rightPlayer.ChooseStrike(new Card(), leftPlayer);
        match.Resolve();
        Assert.True(rightPlayer.Health == 2);
        leftPlayer.ChooseParry(new Card());
        rightPlayer.ChooseStrike(new Card(), leftPlayer);
        match.Resolve();
        Assert.True(rightPlayer.Health == 0);
        Assert.True(rightPlayer.Dead);
    }

    [Test]
    public void BasketStartsWith3Health()
    {
        Assert.True(match.Basket.Health == 3);
    }

    [Test]
    public void BasketStartsInSquare()
    {
        Assert.True(match.Basket.Place == Place.Square);
    }

    [Test]
    public void CanStrikeBasketFromSamePlace()
    {
        leftPlayer.ChooseMove(new Card(), Place.Square, rightPlayer);
        rightPlayer.ChooseMove(new Card(), Place.Square, leftPlayer);
        match.Resolve();
        leftPlayer.ChooseStrike(new Card(), match.Basket);
        rightPlayer.ChooseMove(new Card(), Place.Square, rightPlayer);
        match.Resolve();
        Assert.True(match.Basket.Health == 2);
    }

    [Test]
    public void CanMoveBasketFromSamePlace()
    {
        leftPlayer.ChooseMove(new Card(), Place.Square, leftPlayer);
        rightPlayer.ChooseMove(new Card(), Place.Square, rightPlayer);
        match.Resolve();
        leftPlayer.ChooseMove(new Card(), Place.Curtain, match.Basket);
        rightPlayer.ChooseMove(new Card(), Place.Square, rightPlayer);
        match.Resolve();
        Assert.True(match.Basket.Place == Place.Curtain);
    }

    [Test]
    public void CannotStrikeOneself()
    {
        leftPlayer.ChooseStrike(new Card(), leftPlayer);
        rightPlayer.ChooseMove(new Card(), Place.Square, rightPlayer);
        match.Resolve();
        Assert.True(leftPlayer.Health == 3);
    }

    [Test]
    public void CannotMoveOpponent()
    {
        leftPlayer.ChooseMove(new Card(), Place.Square, leftPlayer);  
        rightPlayer.ChooseParry(new Card());
        match.Resolve();
        Assert.True(rightPlayer.Place == Place.Steps);
    }
}