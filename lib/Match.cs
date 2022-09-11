using System;
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
            this.seed = challenge.Host.Seed ^ challenge.Challenger.Seed;
            this.host = challenge.Host;
            this.challenger = challenge.Challenger;
            this.Active = true;
            this.Challenge = challenge;
            this.generator = Generator.FromSeed(seed);
            this.Basket = new Basket(this.generator, GetFirstPlayerIn);
            host.Deck = (challenge.Host.Deck as SealedDeck)?.Unsealed() ?? Deck.Random(this.generator);
            challenger.Deck = challenge.Challenger.Deck ?? Deck.Random(this.generator);
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

        public void Start()
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
        private bool bVerbose;

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
                Basket.RollFace();
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
            _burnActiveCard(host.ActivePlay);
            _burnActiveCard(challenger.ActivePlay);
            if (Basket.ActivePlay != null)
            {
                _burnActiveCard(Basket.ActivePlay);
            }
        }

        private void _burnActiveCard(Play activePlay)
        {
            var targetPlay = activePlay.Target.ActivePlay;
            if (activePlay.Card.Platonic == burns[targetPlay.Card.Platonic])
            {
                if (bVerbose)
                {
                    Console.WriteLine($"{targetPlay.Card.Platonic.ToString()} burns {activePlay.Card.Platonic.ToString()}");
                }

                activePlay.Card.Burned = true;
                activePlay.Card.OnBurned?.Invoke(host);
            }
        }

        private void processPlays()
        {
            if (bVerbose)
            {
                Console.WriteLine();
            }

            _moveStrikeParry(host);
            _moveStrikeParry(challenger);
            _resolvePlay(host);
            _resolvePlay(challenger);
        }

        private void _resolvePlay(PlayerController player)
        {
            var activePlay = player.ActivePlay;
            var opposedPlay = player.ActivePlay.Target.ActivePlay;
            if (activePlay.BigEnough(opposedPlay.EffectivePower)
                && !activePlay.IsStoppedBy(opposedPlay.Choice))
            {
                if (bVerbose)
                {
                    Console.WriteLine(
                        $"{activePlay.Card.Choice.ToString()} {activePlay.Card.Power.ToString()} " +
                        $"resolves against " +
                        $"{opposedPlay.Card.Choice.ToString()} {opposedPlay.Card.Power.ToString()}");
                }
                activePlay.Resolve(player);
            }
        }

        private void _moveStrikeParry(PlayerController player)
        {
            if (player.ActivePlay.BeatsInMoveStrikeParry(player.ActivePlay.Target.ActivePlay.Choice))
            {
                if (bVerbose)
                {
                    Console.WriteLine($"{player.ActivePlay.Card.Choice.ToString()} card wins hand");
                }
                player.ActivePlay.Card.OnWinHand?.Invoke(player);
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

        public void SetVerbose(bool verbose)
        {
            bVerbose = verbose;
        }

        public Target GetFirstPlayerIn(Place location)
        {
            var hostTimeInLocation = host.Location == location ? host.Residency : 0;
            var challengerTimeInLocation = challenger.Location == location ? challenger.Residency : 0;

            if (challengerTimeInLocation > hostTimeInLocation)
            {
                return challenger;
            }

            if (hostTimeInLocation > 0)
            {
                return host;
            }

            return null;
        }
    }
}