using System.Collections.Generic;

namespace lib
{
    public class SealedDeck 
    {
        private Deck internalDeck;
        public SealedDeck(Deck deck)
        {
            this.internalDeck = deck;
        }
        public Deck Unsealed()
        {
            return internalDeck;
        }
    }
}