namespace lib
{
    public class Adverb
    {
        private static Adverb[] adverbs = new Adverb[]
        {
            new Adverb(){Name = "Earnestly"},
            new Adverb(){Name = "Impressively"},
            new Adverb(){Name = "Nearby"},
            new Adverb(){Name = "Tenaciously"},
            new Adverb(){Name = "Gratifyingly"},
            new Adverb(){Name = "Sometimes"},
            new Adverb(){Name = "Hopefully"},
            new Adverb(){Name = "Backwards"}
        };
        public string Name { get; private set; }

        public static Adverb Get(uint index)
        {
            return adverbs[index % adverbs.Length];
        }
    }
}