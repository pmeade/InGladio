using System;

namespace lib
{
    public class Generator
    {
        public static Generator FromSeed(uint seed)
        {
            if (seed == UInt32.MaxValue)
            {
                return new Generator() { Valid = false };
            }
            
            var generator = new Generator { Seed = seed, Valid = true};
            generator.BaseCardIndex = seed;
            return generator;
        }

        public uint Seed { get; private set; }
        
        public uint BaseCardIndex { get; private set; }
        public bool Valid { get; private set; }

        public Card Roll()
        {
            if (!Valid)
            {
                return null;
            }

            var card = new Card();
            card.Platonic = (Platonic)(nextRoll(card) % 3);
            var powerRoll = nextRoll(card) % 16;
            card.Power = (powerRoll > 11) ? Power.Eight : powerRoll > 7 ? Power.Five : Power.Three;
            card.Adjective = Adjective.Get(nextRoll(card));
            card.Adverb = (card.Adjective != null) ? Adverb.Get(nextRoll(card)) : null;
            card.Origin = Origin.Get(nextRoll(card));
            card.Meta = (card.Origin != null) ? Meta.Get(nextRoll(card)):null;


            return card;
        }

        private uint nextRoll(Card card)
        {
            BaseCardIndex += (uint)card.GetHashCode();
            return BaseCardIndex;
        }
    }
}