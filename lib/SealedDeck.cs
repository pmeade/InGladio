using System.Collections.Generic;

namespace lib
{
    public class SealedDeck : Deck
    {
        private Deck internalDeck;
        public SealedDeck(Deck deck) : base(deck.cards)
        {
            this.internalDeck = deck;
        }
        public Deck Unsealed()
        {
            return internalDeck;
        }
    }
}