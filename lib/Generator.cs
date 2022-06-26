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

            CardClass cardClass = null;
            Origin origin = null;
            
            var platonic = (Platonic)(BaseCardIndex % 3);

            var initialRoll = BaseCardIndex % 16;
            if (initialRoll > 7)
            {
                if (initialRoll % 2 == 0)
                {
                    cardClass = CardClass.Get(BaseCardIndex);
                }
                else
                {
                    origin = Origin.Get(BaseCardIndex);
                }
            }

            Adverb adverb = null;
            Meta meta = null;
            if (initialRoll > 11)
            {
                if (initialRoll % 2 == 0)
                {
                    adverb = Adverb.Get(BaseCardIndex);
                }
                else
                {
                    meta = Meta.Get(BaseCardIndex);
                }
            }

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