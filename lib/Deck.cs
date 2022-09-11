using System;
using System.Collections.Generic;

namespace lib
{
    public class Deck
    {
        public int Count { get; private set; }
        
        protected bool Equals(Deck other)
        {
            return Equals(cards, other.cards);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Deck)obj);
        }

        public override int GetHashCode()
        {
            return (cards != null ? cards.GetHashCode() : 0);
        }

        private const int DefaultDeckSize = 8;
        protected internal Card[] cards;

        protected Deck(Card[] cards)
        {
            this.cards = new Card[DefaultDeckSize];
            
            for (int i = 0; i < DefaultDeckSize; ++i)
            {
                if (i < cards.Length)
                {
                    this.cards[i] = cards[i];
                }
                else
                {
                    this.cards[i] = new Card();
                }
            }

            Count = DefaultDeckSize;
        }
        public virtual Card Get(uint index)
        {
            if (index < Count)
            {
                return cards[index];
            }

            throw new ArgumentOutOfRangeException();
        }

        public SealedDeck Sealed()
        {
            return new SealedDeck(this);
        }

        public static Deck FromCards(Card[] chosenCards)
        {
            return new Deck(chosenCards);
        }

        public static Deck Random(Generator generator)
        {
            return new Deck(new Card[]
            {
                generator.Roll(1),
                generator.Roll(1),
                generator.Roll(1),
                generator.Roll(2),
                generator.Roll(2),
                generator.Roll(3),
                generator.Roll(4),
                generator.Roll(5)
            });
        }

        public Card Play(uint index)
        {
            return Get(index);
        }
    }
}