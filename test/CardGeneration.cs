using lib;

namespace test;

public class CardGeneration
{
    [Test]
    public void SeedGreaterThanOrEqualToZero()
    {
        var validGenerator = Generator.FromSeed(0);
        Assert.True(validGenerator.Valid);
        Assert.IsNotNull(validGenerator.Valid);
    }

    [Test]
    public void SeedLessThan0xffffffff()
    {
        var generatorMax = Generator.FromSeed(UInt32.MaxValue);
        Assert.False(generatorMax.Valid);
        Assert.IsNull(generatorMax.Roll());
    }
    
    [Test]
    public void SeedPlusOneActsAsJumpValueForIteratingDomain()
    {
        var generator0 = Generator.FromSeed(0);
        Assert.IsTrue(generator0.BaseCardIndex == 0);
        generator0.Roll();
        Assert.IsTrue(generator0.BaseCardIndex == 1);

        var generator222 = Generator.FromSeed(221);
        generator222.Roll();
        Assert.IsTrue(generator222.BaseCardIndex == 443);
    }

    //[Test]
    public void CyclesThroughEightClassesOfCard()
    {
        var generator = Generator.FromSeed(0);
        var classes = new HashSet<CardClass>();
        for (int i = 0; i < 8; ++i)
        {
            var newCardClass = generator.Roll().CardClass;
            Assert.IsFalse(classes.Contains(newCardClass));
            classes.Add(newCardClass);
            Assert.IsTrue(classes.Contains(newCardClass));
        }

        var ninthClass = generator.Roll().CardClass;
        Assert.IsTrue(classes.Contains(ninthClass));
    }

    //[Test]
    public void CyclesThroughEightOrigins()
    {
        var generator = Generator.FromSeed(0);
        var origins = new HashSet<Origin>();
        for (int i = 0; i < 32; ++i)
        {
            var newOrigin = generator.Roll().Origin;
            if (newOrigin != null)
            {
                Assert.IsFalse(origins.Contains(newOrigin));
                origins.Add(newOrigin);
                Assert.IsTrue(origins.Contains(newOrigin));
            }
        }

        Assert.That(origins.Count, Is.EqualTo(8));
    }

    [Test]
    public void CyclesThroughThreePlatonics()
    {
        var generator = Generator.FromSeed(0);
        var platonics = new HashSet<Platonic>();
        for (int i = 0; i < 3; ++i)
        {
            var platonic = generator.Roll().Platonic;
            Assert.IsFalse(platonics.Contains(platonic));
            platonics.Add(platonic);
            Assert.IsTrue(platonics.Contains(platonic));
        }
    
        var fourthPlatonic = generator.Roll().Platonic;
        Assert.IsTrue(platonics.Contains(fourthPlatonic));
    }
    
    [Test]
    public void EighthOfCardsHaveAnAdverb()
    {
        var generator = Generator.FromSeed(0);
        List<Card> withAdverbs = new List<Card>();
        for (int i = 0; i < 64; ++i)
        {
            var card = generator.Roll();
            if (card.Adverb != null)
            {
                withAdverbs.Add(card);
            }
        }
        
        Assert.That(withAdverbs.Count, Is.EqualTo(8));
    }

    [Test]
    public void EightOfCardsHaveAMetaComponent()
    {
        var generator = Generator.FromSeed(0);
        List<Card> withMeta = new List<Card>();
        for (int i = 0; i < 64; ++i)
        {
            var card = generator.Roll();
            if (card.Meta != null)
            {
                withMeta.Add(card);
            }
        }
        
        Assert.IsTrue(withMeta.Count == 8);
    }
}