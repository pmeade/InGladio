using lib;
using lib.CardExtensionMethods;

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
        this.match = leftPlayer.StartMatch(true);
    }

    [Test]
    public void PlayerCanPlayCardFromDeck()
    {
        Assert.True(leftPlayer.CanPlay());
        var deck = Deck.Random(Generator.FromSeed(1));
        leftPlayer.MoveSelf(deck.Get(0), Place.Square);
        Assert.False(leftPlayer.CanPlay());
    }

    [Test]
    public void RockBurnsScissors()
    {
        leftPlayer.StrikeOpponent(Card.BasicStrike().AsRock());
        rightPlayer.StrikeOpponent(Card.BasicStrike().AsScissors());
        match.Resolve();
        var board = match.Board;
        Assert.False(leftPlayer.ActivePlay.Card.Burned);
        Assert.True(rightPlayer.ActivePlay.Card.Burned);
    }
    
    [Test]
    public void ScissorsBurnsPaper()
    {
        leftPlayer.StrikeOpponent(Card.BasicStrike().AsScissors());
        rightPlayer.StrikeOpponent(Card.BasicStrike().AsPaper());
        match.Resolve();
        var board = match.Board;
        Assert.False(leftPlayer.ActivePlay.Card.Burned);
        Assert.True(rightPlayer.ActivePlay.Card.Burned);
    }
    
    [Test]
    public void PaperBurnsRock()
    {
        var leftCard = new Card(){Platonic = Platonic.Paper};
        leftPlayer.MoveSelf(leftCard, lib.Place.Square);
        rightPlayer.MoveSelf(Card.BasicMove().AsRock(), Place.Square);
        var board = match.Board;
        match.Resolve();
        Assert.False(leftPlayer.ActivePlay.Card.Burned);
        Assert.True(rightPlayer.ActivePlay.Card.Burned);
    }

    [Test]
    public void MoveSucceedsVsMove()
    {
        var board = match.Board;
        Assert.True(leftPlayer.Place == Place.Steps);
        leftPlayer.MoveSelf(new Card(), Place.Square);
        rightPlayer.MoveSelf(new Card(), Place.Square);
        match.Resolve();
        Assert.True(leftPlayer.Place == Place.Square);
    }

    [Test]
    public void MoveSucceedsVsParry()
    {
        var board = match.Board;
        Assert.True(leftPlayer.Place == Place.Steps);
        leftPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        rightPlayer.ParryOpponent(Card.BasicParry());
        match.Resolve();
        Assert.True(leftPlayer.Place == Place.Square);
    }

    [Test]
    public void MoveFailsVsStrike()
    {
        var board = match.Board;
        Assert.True(leftPlayer.Place == Place.Steps);
        leftPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        rightPlayer.StrikeOpponent(Card.BasicStrike());
        match.Resolve();
        Assert.True(leftPlayer.Place == Place.Steps);
    }
    
    [Test]
    public void StrikeSucceedsVsStrike()
    {
        Assert.True(rightPlayer.Health == 3);
        leftPlayer.StrikeOpponent(Card.BasicStrike());
        rightPlayer.StrikeOpponent(Card.BasicStrike());
        match.Resolve();
        Assert.True(rightPlayer.Health == 2);
    }
    
    [Test]
    public void StrikeSucceedsVsMove()
    {
        Assert.True(rightPlayer.Health == 3);
        leftPlayer.StrikeOpponent(Card.BasicStrike());
        rightPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        match.Resolve();
        Assert.True(rightPlayer.Health == 2);
    }
    
    [Test]
    public void StrikeFailsVsParry()
    {
        Assert.True(rightPlayer.Health == 3);
        leftPlayer.StrikeOpponent(Card.BasicStrike());
        rightPlayer.ParryOpponent(Card.BasicParry());
        match.Resolve();
        Assert.True(rightPlayer.Health == 3);
    }

    [Test]
    public void ParrySucceedsVsStrike()
    {
        Assert.True(rightPlayer.Health == 3);
        leftPlayer.ParryOpponent(Card.BasicParry());
        rightPlayer.StrikeOpponent(Card.BasicStrike());
        match.Resolve();
        Assert.True(rightPlayer.Health == 1);
    }

    [Test]
    public void ParryFailsVsMove()
    {
        Assert.True(rightPlayer.Health == 3);
        rightPlayer.ParryOpponent(Card.BasicParry());
        rightPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        match.Resolve();
        Assert.True(rightPlayer.Health == 3);
    }

    [Test]
    public void ParryFailsVsParry()
    {
        Assert.True(rightPlayer.Health == 3);
        rightPlayer.ParryOpponent(Card.BasicParry());
        rightPlayer.ParryOpponent(Card.BasicParry());
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
        leftPlayer.StrikeOpponent(Card.BasicStrike());
        rightPlayer.StrikeOpponent(Card.BasicStrike());
        match.Resolve();
        Assert.True(rightPlayer.Health == 2);
        leftPlayer.ParryOpponent(Card.BasicParry());
        rightPlayer.StrikeOpponent(Card.BasicStrike());
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
    public void CanParryBasketFromSamePlace()
    {
        leftPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        rightPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        match.Resolve();

        bool parried = false;

        while (!parried)
        {
            if (match.Basket.Face.Choice == Choice.Strike)
            {
                leftPlayer.ParryBasket(Card.BasicParry().AsEight());
                rightPlayer.MoveSelf(Card.BasicMove(), Place.Square);
                parried = true;
            }
            else
            {
                leftPlayer.MoveSelf(Card.BasicMove(), Place.Square);
                rightPlayer.MoveSelf(Card.BasicMove(), Place.Square);
            }

            match.Resolve();
        }

        Assert.True(match.Basket.Health == 1);
    }

    [Test]
    public void CanStrikeBasketFromSamePlace()
    {
        leftPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        rightPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        match.Resolve();

        bool struck = false;

        while (!struck)
        {
            if (match.Basket.Face.Choice == Choice.Move)
            {
                leftPlayer.StrikeBasket(Card.BasicStrike().AsEight());
                rightPlayer.MoveSelf(Card.BasicMove(), Place.Square);
                struck = true;
            }
            else
            {
                leftPlayer.MoveSelf(Card.BasicMove(), Place.Square);
                rightPlayer.MoveSelf(Card.BasicMove(), Place.Square);
            }

            match.Resolve();
        }

        Assert.True(match.Basket.Health == 2);
    }

    [Test]
    public void CanMoveBasketFromSamePlace()
    {
        leftPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        rightPlayer.MoveSelf(Card.BasicMove(), Place.Square);
        match.Resolve();

        bool movedBasket = false;

        while (!movedBasket)
        {
            if (match.Basket.Face.Choice == Choice.Parry)
            {
                leftPlayer.MoveBasket(Card.BasicMove().AsEight(), Place.Curtain);
                rightPlayer.MoveSelf(Card.BasicMove(), Place.Square);
                movedBasket = true;
            }
            else
            {
                leftPlayer.MoveSelf(Card.BasicMove(), Place.Square);
                rightPlayer.MoveSelf(Card.BasicMove(), Place.Square);
            }

            match.Resolve();
        }

        Assert.True(match.Basket.Place == Place.Curtain);
    }

    [Test]
    public void CannotStrikeOneself()
    {
        leftPlayer.StrikeOpponent(new Card());
        rightPlayer.MoveSelf(new Card(), Place.Square);
        match.Resolve();
        Assert.True(leftPlayer.Health == 3);
    }

    [Test]
    public void CannotMoveOpponent()
    {
        leftPlayer.MoveSelf(new Card(), Place.Square);  
        rightPlayer.ParryOpponent(new Card());
        match.Resolve();
        Assert.True(rightPlayer.Place == Place.Steps);
    }
}