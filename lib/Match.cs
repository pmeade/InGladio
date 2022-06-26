using System.Collections.Generic;

namespace lib
{
    public class Match
    {
        private PlayerController leftPlayer;
        private PlayerController rightPlayer;
        private Play leftPlay;
        private Play rightPlay;

        private Match(Challenge challenge, PlayerController acceptingPlayer)
        {
            this.leftPlayer = challenge.Player;
            this.rightPlayer = acceptingPlayer;
            this.Active = true;
            this.Challenge = challenge;
            this.Basket = new Basket(new LootTable());
        }

        protected Match()
        {
            this.Active = false;
        }

        public bool Active { get; set; }
        public Challenge Challenge { get; private set; }
        public bool Started { get; set; }
        public Board Board { get; set; }
        public Basket Basket { get; }
        public bool Complete { get; private set; }

        public void Play(PlayerController player, Play play)
        {
            if (!CanPlay(player))
            {
                return;
            }

            if (player == leftPlayer)
            {
                leftPlay = play;
            }

            if (player == rightPlayer)
            {
                rightPlay = play;
            }
        }

        public bool CanPlay(PlayerController playerController)
        {
            return (playerController == leftPlayer && leftPlay == null
                    || playerController == rightPlayer && rightPlay == null);
        }

        public void Start(PlayerController playerController)
        {
            if (playerController == leftPlayer && Active && !Started)
            {
                Started = true;
                Board = new Board();
            }
        }

        private Dictionary<Platonic, Platonic> burns = new Dictionary<Platonic, Platonic>()
        {
            { Platonic.Rock, Platonic.Scissors },
            { Platonic.Scissors, Platonic.Paper },
            { Platonic.Paper, Platonic.Rock }
        };

        private Dictionary<Choice, Choice> stops = new Dictionary<Choice, Choice>()
        {
            { Choice.Strike, Choice.Move },
            { Choice.Parry, Choice.Strike }
        };

        public Deck LeftDeck { get; private set; }
        public Deck RightDeck { get; private set; }

        public void Resolve()
        {
            if (leftPlay != null && rightPlay != null)
            {
                Round++;
                Board.LeftCard = leftPlay?.Card;
                Board.RightCard = rightPlay?.Card;
                if (Active && Started && leftPlay != null && rightPlay != null)
                {
                    processPlays();
                    burnCards();
                    leftPlay = null;
                    rightPlay = null;
                }

                checkForVictory();
                Basket.damageDealers.Clear();
            }
        }

        public int Round { get; private set; } = 0;

        private void checkForVictory()
        {
            Basket.CheckForVictory();

            if (leftPlayer.Dead && !rightPlayer.Dead)
            {
                Active = false;
                Complete = true;
                Winner = rightPlayer;
                rightPlayer.Win();
                leftPlayer.Lose();
                Basket.Claim(rightPlayer);
            }
            else if (rightPlayer.Dead && !leftPlayer.Dead)
            {
                Active = false;
                Complete = true;
                Winner = leftPlayer;
                leftPlayer.Win();
                rightPlayer.Lose();
                Basket.Claim(leftPlayer);
            }
            else if (rightPlayer.Dead && leftPlayer.Dead)
            {
                Active = false;
                Complete = true;
                leftPlayer.Lose();
                rightPlayer.Lose();
            }
            else if (Basket.SnatchedBy == leftPlayer)
            {
                Active = false;
                Complete = true;
                Winner = leftPlayer;
                leftPlayer.Win();
                rightPlayer.Lose();
                Basket.Claim(leftPlayer);
            }
            else if (Basket.SnatchedBy == rightPlayer)
            {
                Active = false;
                Complete = true;
                Winner = rightPlayer;
                rightPlayer.Win();
                leftPlayer.Lose();
                Basket.Claim(rightPlayer);
            }
            else if (Round == 8)
            {
                Active = false;
                Complete = true;
                rightPlayer.Lose();
                leftPlayer.Lose();
            }
        }

        public PlayerController Winner { get; private set; }

        private void burnCards()
        {
            if (leftPlay.Card.Platonic == burns[rightPlay.Card.Platonic])
            {
                leftPlay.Card.OnBurned?.Invoke(leftPlayer);
                Board.LeftCard = null;
            }

            if (rightPlay.Card.Platonic == burns[leftPlay.Card.Platonic])
            {
                rightPlay.Card.OnBurned?.Invoke(rightPlayer);
                Board.RightCard = null;
            }
        }

        private void processPlays()
        {
            if (leftPlay.Wins(rightPlay))
            {
                leftPlay.Card.OnWinHand?.Invoke(leftPlayer);
            }

            if (rightPlay.Wins(leftPlay))
            {
                rightPlay.Card.OnWinHand?.Invoke(rightPlayer);
            }
            
            if (leftPlay.BigEnough(rightPlay) && !leftPlay.IsStoppedBy(rightPlay.Choice))
            {
                leftPlay.Resolve(leftPlayer);
            }

            if (rightPlay.BigEnough(leftPlay) && !rightPlay.IsStoppedBy(leftPlay.Choice))
            {
                rightPlay.Resolve(rightPlayer);
            }
        }

        private class PlayerState
        {
            public uint Health { get; set; }
            public Place Place { get; set; } 
        }

        public static Match Prebuilt(Challenge challenge, PlayerController acceptingPlayer, Deck acceptingDeck)
        {
            return new Match(challenge, acceptingPlayer)
            {
                LeftDeck = challenge.Deck.Unsealed(),
                RightDeck = acceptingDeck
            };
        }

        public static Match Random(Challenge challenge, PlayerController acceptingPlayer)
        {
            return new Match(challenge, acceptingPlayer)
            {
                LeftDeck = Deck.Random(),
                RightDeck = Deck.Random()
            };
        }
    }
}