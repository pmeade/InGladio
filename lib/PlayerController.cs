using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace lib
{
    public class PlayerController : Target
    {
        private Challenge challenge = null;
        
        private Match match = null;
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

        public void Move(Place place)
        {
            Place = place;
        }

        public Place Location => Place;

        public void ChooseMove(Card card, Place place, Target target, Place locationPlayedFrom)
        {
            if (target != Opponent)
            {
                Play(lib.Play.Move(card, place, target, locationPlayedFrom));
            }
        }
        public void ChooseParry(Card card, Place locationPlayedFrom)
        {
            Play(lib.Play.Parry(card, Opponent, locationPlayedFrom));
        }

        public void ChooseStrike(Card card, Target target, Place locationPlayedFrom)
        {
            if (target != this)
            {
                Play(lib.Play.Strike(card, target, locationPlayedFrom));
            }
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
            var card = Deck.Get(cardIndex);
            switch (card.Choice)
            {
                case Choice.Move:
                    Place moveLocation;
                    Place.TryParse(data[0], true, out moveLocation);
                    ChooseMove(card, moveLocation, this, Place);
                    break;
                
                case Choice.Parry:
                    ChooseParry(card, Place);
                    break;
                
                case Choice.Strike:
                    var target = (data[0].Equals("basket")) ? match.Basket : Opponent as Target;
                    ChooseStrike(card, target, Place);
                    break;
            }
        }
    }
}