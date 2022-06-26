namespace lib
{
    public class Adverb
    {
        private static Adverb[] adverbs = new Adverb[] { new Adverb()};
        public static Adverb Get(uint index)
        {
            return adverbs[index % adverbs.Length];
        }
    }
}