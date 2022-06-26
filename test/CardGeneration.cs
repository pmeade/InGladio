using lib;

namespace test;

public class CardGeneration
{
    [Test]
    public void ThereAreEightClassesOfCard()
    {
        var seed = 0;
        var generator = Generator.From(seed);
        var card1 = generator.Roll();
        
    }
}