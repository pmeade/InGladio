// See https://aka.ms/new-console-template for more information

using System.Text;
using lib;

Console.WriteLine("cardgen");

var generator = Generator.FromSeed((uint)DateTime.Now.Millisecond);

var count = args.Length > 1 ? int.Parse(args[1]) : 64;

for (int i = 0; i < count; ++i)
{
    var card = generator.Roll();
    StringBuilder stringBuilder = new StringBuilder();
    
    if (card.Adverb != null)
    {
        stringBuilder.Append(card.Adverb.Name + " ");
    }

    if (card.Adjective != null)
    {
        stringBuilder.Append(card.Adjective.Name + " ");
    }

    stringBuilder.Append(card.Platonic.ToString() + " ");

    if (card.Origin != null)
    {
        stringBuilder.Append("of the ");
    }

    if (card.Meta != null)
    {
        stringBuilder.Append(card.Meta.Name + " ");
    }

    if (card.Origin != null)
    {
        stringBuilder.Append(card.Origin.Name);
    }

    stringBuilder.Append(" (" + card.Power.ToString() + ")");

    Console.WriteLine(stringBuilder.ToString());
}
