using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using lib;

namespace client;

public class CardLibrary
{
    public List<Card> Cards;

    public static CardLibrary Load(string cardFile)
    {
        var stream = File.Open(cardFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        stream.Close();
        var loadedCards = new List<Card>();
        var lines = File.ReadAllLines(cardFile);
        foreach (var line in lines)
        {
            loadedCards.Add(parseCard(line));
        }

        return new CardLibrary() { Cards = loadedCards };
    }

    public void Save(string cardFile)
    {
        var lines = new List<string>();
        foreach (var card in Cards)
        {
            lines.Add(card.ToString());
        }
        
        File.WriteAllLines(cardFile, lines);
    }

    private static Card parseCard(string line)
    {
        return Card.FromString(line);
    }

    public void Generate(int numCardsToGenerate)
    {
        var generator = Generator.FromSeed((int)(DateTime.Now.Ticks & 0x0000ffff));
        for (int i = 0; i < numCardsToGenerate; ++i)
        {
            Cards.Add(generator.Roll());
        }
    }
}