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

        public Card Roll()
        {
            if (!Valid)
            {
                return null;
            }

            var card = new Card();

            card.Choice = (Choice)(nextRoll(card) % 3);
            card.Platonic = (Platonic)(nextRoll(card) % 3);
            var powerRoll = nextRoll(card) % 16;
            
            card.Power = (powerRoll > 13) ? Power.Eight : powerRoll > 9 ? Power.Five : Power.Three;

            if (nextRoll(card) % 4 == 0)
            {
                card.Adjective = Adjective.Get(nextRoll(card));

                if (nextRoll(card) % 6 == 0)
                {
                    card.Adverb = Adverb.Get(nextRoll(card));
                }
            }
            
            if (nextRoll(card) % 6 == 0)
            {
                card.Origin = Origin.Get(nextRoll(card));
                if (nextRoll(card) % 8 == 0)
                {
                    card.Meta = Meta.Get(nextRoll(card));
                }
            }


            return card;
        }

        private uint nextRoll(Card card)
        {
            return (uint)random.Next();
        }
    }
}