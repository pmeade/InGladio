using System.Collections.Generic;

namespace lib
{
    public class Match
    {
        private PlayerController host;
        private PlayerController challenger;
        private Play leftPlay;
        private Play rightPlay;
        private Generator generator;
        public Deck LeftDeck => host.Deck;
        public Deck RightDeck => challenger.Deck;

        private Match(Challenge challenge)
        {
            this.seed = challenge.Host.Seed ^ challenge.challenger.Seed;
            this.host = challenge.Host;
            this.challenger = challenge.challenger;
            this.Active = true;
            this.Challenge = challenge;
            this.generator = Generator.FromSeed(seed);
            this.Basket = new Basket(this.generator);
            host.Deck = (challenge.Host.Deck as SealedDeck)?.Unsealed() ?? Deck.Random(this.generator);
            challenger.Deck = challenge.challenger.Deck ?? Deck.Random(this.generator);
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

            if (player == host)
            {
                leftPlay = play;
            }

            if (player == challenger)
            {
                rightPlay = play;
            }
        }

        public bool CanPlay(PlayerController playerController)
        {
            return (playerController == host && leftPlay == null
                    || playerController == challenger && rightPlay == null);
        }

        public void Start(PlayerController playerController)
        {
            Started = true;
            Board = new Board();
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

        private readonly int seed;

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

            if (host.Dead && !challenger.Dead)
            {
                Active = false;
                Complete = true;
                Winner = challenger;
                challenger.Win();
                host.Lose();
                Basket.Claim(challenger);
            }
            else if (challenger.Dead && !host.Dead)
            {
                Active = false;
                Complete = true;
                Winner = host;
                host.Win();
                challenger.Lose();
                Basket.Claim(host);
            }
            else if (challenger.Dead && host.Dead)
            {
                Active = false;
                Complete = true;
                host.Lose();
                challenger.Lose();
            }
            else if (Basket.SnatchedBy == host)
            {
                Active = false;
                Complete = true;
                Winner = host;
                host.Win();
                challenger.Lose();
                Basket.Claim(host);
            }
            else if (Basket.SnatchedBy == challenger)
            {
                Active = false;
                Complete = true;
                Winner = challenger;
                challenger.Win();
                host.Lose();
                Basket.Claim(challenger);
            }
            else if (Round == 8)
            {
                Active = false;
                Complete = true;
                challenger.Lose();
                host.Lose();
            }
        }

        public PlayerController Winner { get; private set; }

        private void burnCards()
        {
            if (leftPlay.Card.Platonic == burns[rightPlay.Card.Platonic])
            {
                leftPlay.Card.OnBurned?.Invoke(host);
                Board.LeftCard = null;
            }

            if (rightPlay.Card.Platonic == burns[leftPlay.Card.Platonic])
            {
                rightPlay.Card.OnBurned?.Invoke(challenger);
                Board.RightCard = null;
            }
        }

        private void processPlays()
        {
            if (leftPlay.Wins(rightPlay))
            {
                leftPlay.Card.OnWinHand?.Invoke(host);
            }

            if (rightPlay.Wins(leftPlay))
            {
                rightPlay.Card.OnWinHand?.Invoke(challenger);
            }
            
            if (leftPlay.BigEnough(rightPlay) && !leftPlay.IsStoppedBy(rightPlay.Choice))
            {
                leftPlay.Resolve(host);
            }

            if (rightPlay.BigEnough(leftPlay) && !rightPlay.IsStoppedBy(leftPlay.Choice))
            {
                rightPlay.Resolve(challenger);
            }
        }

        public static Match Create(Challenge challenge)
        {
            return new Match(challenge);
        }

        public PlayerController GetHost()
        {
            return host;
        }

        public PlayerController GetChallenger()
        {
            return challenger;
        }
    }
}