using System;

namespace lib
{
    public class Generator
    {
        public static Generator From(int seed)
        {
            return new Generator() { Seed = seed };
        }

        public int Seed { get; private set; }

        public Card Roll()
        {
            return new Card();
        }
    }
}