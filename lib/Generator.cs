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
            
            var cardClass = (CardClass)(BaseCardIndex % 8);
            var origin = (Origin)(BaseCardIndex % 8);
            var platonic = (Platonic)(BaseCardIndex % 3);

            var initialRoll = BaseCardIndex % 16;
            var adverb = (initialRoll > 7) ? Adverb.Get(BaseCardIndex):null;
            var meta = (initialRoll > 12) ? Meta.Get(BaseCardIndex) : null;
            
            BaseCardIndex += (Seed + 1);
            return new Card()
            {
                CardClass = cardClass, 
                Origin = origin,
                Platonic = platonic, 
                Adverb = adverb,
                Meta = meta
            };
        }
    }
}