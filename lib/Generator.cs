using System;

namespace lib
{
    public class Generator
    {
        public static Generator FromSeed(int seed)
        {
            var generator = new Generator { Valid = true};
            generator.random = new Random(seed);
            return generator;
        }

        private Random random;
        public bool Valid { get; private set; }

        public Card Roll(int bonusTries = 0)
        {
            if (!Valid)
            {
                return null;
            }

            var card = new Card();

            card.Choice = (Choice)(nextRoll() % 3);
            card.Platonic = (Platonic)(nextRoll() % 3);
            card.Power = Power.Three;

            while (bonusTries > 0)
            {
                if (card.Power == Power.Three && nextRoll()%16 > 9)
                {
                    card.Power = Power.Five;
                    --bonusTries;
                    continue;
                }

                if (card.Power == Power.Five && nextRoll()%16 > 13)
                {
                    card.Power = Power.Eight;
                    --bonusTries;
                    continue;
                }
                
                if (card.Adjective == null && nextRoll() % 4 == 0)
                {
                    card.Adjective = Adjective.Get(nextRoll());
                    --bonusTries;
                    continue;
                }

                if (card.Adjective != null && card.Adverb == null && nextRoll() % 6 == 0)
                {
                    card.Adverb = Adverb.Get(nextRoll());
                    --bonusTries;
                    continue;
                }

                if (card.Origin == null && nextRoll() % 6 == 0)
                {
                    card.Origin = Origin.Get(nextRoll());
                    --bonusTries;
                    continue;
                }

                if (card.Origin != null && card.Meta == null && nextRoll() % 8 == 0)
                {
                    card.Meta = Meta.Get(nextRoll());
                    --bonusTries;
                    continue;
                }

                --bonusTries;
            }

            return card;
        }

        private uint nextRoll()
        {
            return (uint)random.Next();
        }

        public int Next()
        {
            return random.Next();
        }
    }
}