using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace lib
{
    public class PlayerController : Target, Mover
    {
        private Challenge challenge = null;
        
        private Match match = null;
        
        public int Residency = 1;
        public Place Place { get; set; }
        public int Health { get; private set; } = 3;
        public bool Dead { get; private set; } = false;
        
        public PlayerController Opponent { get; private set; }

        public Challenge CreateChallenge(SealedDeck deck)
        {
            if (challenge == null && deck != null)
            {
                challenge = Challenge.Prebuilt(this, deck);
                return challenge;
            }

            return new FailedChallenge();
        }
        public Challenge CreateChallenge()
        {
            if (challenge == null)
            {
                challenge = Challenge.Random(this);
                return challenge;
            }

            return new FailedChallenge();
        }

        private class FailedChallenge : Challenge
        {
            public FailedChallenge()
            {
            }
        }
        public Match AcceptChallenge(Challenge otherChallenge, Deck myDeck)
        {
            if (challenge != null)
            {
                return new FailedMatch();
            }
            
            if (!otherChallenge.Open)
            {
                return new FailedMatch();
            }

            Deck = myDeck;
            otherChallenge.Accept(this);
            match = Match.Create(otherChallenge);
            if (Deck == null)
            {
                Deck = match.RightDeck;
            }
            otherChallenge.Host.ChallengeAccepted(match);
            return match;
        }

        public Match AcceptChallenge(Challenge otherChallenge)
        {
            if (challenge != null)
            {
                return new FailedMatch();
            }
            
            if (!otherChallenge.Open)
            {
                return new FailedMatch();
            }
            
            otherChallenge.Accept(this);
            match = Match.Create(otherChallenge);
            otherChallenge.Host.ChallengeAccepted(match);
            return match;
        }

        private void ChallengeAccepted(Match match)
        {
            this.match = match;
            this.Opponent = match.Challenge.Challenger;
            match.Challenge.Challenger.Opponent = this;
            Deck = match.LeftDeck;
        }

        private class FailedMatch : Match
        {
        }

        private void Play(Play play)
        {
            ActivePlay = play;
            match?.Play(this, play);
        }

        public bool CanPlay()
        {
            if (match == null)
            {
                return false;
            }

            return match.CanPlay(this);
        }

        public Match StartMatch(bool isHost)
        {
            if (match != null && match.Active && !match.Started)
            {
                // Opponent = isHost ? match.Challenge.Challenger : match.Challenge.Host;
                // Opponent.Opponent = this;
                match.Start();
            }

            return match;
        }

        public void TakeDamage(int amount, PlayerController dealer)
        {
            Health = Math.Max(Health - amount, 0);
            if (Health == 0)
            {
                Dead = true;
            }
        }

        public void UpdateLocation(Place place)
        {
            if (place != Place)
            {
                Place = place;
                Residency = 1;
            }
        }

        public Place Location => Place;
        public Play ActivePlay { get; set; }

        public void MoveSelf(Card card, Place moveTo)
        {
            Play(lib.Play.Move(card, moveTo, Opponent, this, Location));
        }

        public void MoveBasket(Card card, Place moveTo)
        {
            if (Location == match?.Basket.Location)
            {
                Play(lib.Play.Move(card, moveTo, match?.Basket, match?.Basket, Location));
            }
        }
        
        public void ParryOpponent(Card card)
        {
            Play(lib.Play.Parry(card, Opponent, Location));
        }
        
        public void ParryBasket(Card card)
        {
            Play(lib.Play.Parry(card, match?.Basket, Location));
        }

        public void StrikeOpponent(Card card)
        {
            Play(lib.Play.Strike(card, Opponent, Location));
        }
        public void StrikeBasket(Card card)
        {
            Play(lib.Play.Strike(card, match?.Basket, Location));
        }

        public void Win()
        {
            completeMatch();
        }

        public void Lose()
        {
            completeMatch();
        }

        private void completeMatch()
        {
            completedMatches.Add(match);
            match = null;
        }

        public List<Match> completedMatches { get; } = new List<Match>();
        public int Seed { get; set; }
        public Deck Deck { get; set; }

        public void HealSelf(int amount)
        {
            Health = Math.Min(Health + 3, 3);
        }

        public void PlayCard(uint cardIndex, string[] data)
        {
            _accumulateResidency();
            
            var card = Deck.Get(cardIndex);
            Target target;
            bool basket;
            switch (card.Choice)
            {
                case Choice.Move:
                    Enum.TryParse(data[0], true, out Place moveTo);
                    basket = (data[1].Equals("basket"));
                    if (basket)
                    {
                        MoveBasket(card, moveTo);
                    }
                    else
                    {
                        MoveSelf(card, moveTo);
                    }
                    break;
                
                case Choice.Parry:
                    basket = (data[0].Equals("basket"));
                    if (basket)
                    {
                        ParryBasket(card);
                    }
                    else
                    {
                        ParryOpponent(card);
                    }
                    break;
                
                case Choice.Strike:
                    basket = (data[0].Equals("basket"));
                    if (basket)
                    {
                        StrikeBasket(card);
                    }
                    else
                    {
                        StrikeOpponent(card);
                    }
                    break;
            }
        }

        private void _accumulateResidency()
        {
            Residency += 1;
        }
    }

    public interface Mover
    {
        void UpdateLocation(Place place);
    }
}