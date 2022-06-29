using System;
using System.Collections.Generic;
using System.Linq;

namespace lib
{
    public class ClientController
    {
        public List<Card> Cards { get; } = new List<Card>();
        public List<Deck> Decks { get; } = new List<Deck>();

        private PlayerController playerController = new PlayerController();
        
        public event EventHandler ChallengeAccepted;
        public void AddCards(Card[] cards)
        {
            Cards.AddRange(cards);
        }

        public Deck MakeDeck(int[] cardIndexes)
        {
            if (cardIndexes.Length != 8)
            {
                return null;
            }
            
            List<Card> cards = new List<Card>();
            foreach (var index in cardIndexes)
            {
                if (index >= Cards.Count)
                {
                    return null;
                }
                cards.Add(Cards[index]);
            }

            Decks.Add(Deck.FromCards(cards.ToArray()));
            return Decks.Last();
        }

        public Challenge MakeChallenge()
        {
            return _makeChallenge(Deck.Random());
        }

        public Challenge MakeChallenge(int deckIndex)
        {
            if (deckIndex >= Decks.Count || deckIndex < 0)
            {
                return null;
            }

            return _makeChallenge(Decks[deckIndex]);
        }

        private Challenge _makeChallenge(Deck deck)
        {
            var challenge = playerController.CreateChallenge(deck.Sealed());
            challenge.ChallengeAccepted += ChallengeAccepted;
            return challenge;
        }
    }
}