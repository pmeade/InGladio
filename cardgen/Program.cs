// See https://aka.ms/new-console-template for more information

using lib;

Console.WriteLine("cardgen");

var generator = Generator.FromSeed((uint)DateTime.Now.Millisecond);

var count = args.Length > 1 ? int.Parse(args[1]) : 64;

for (int i = 0; i < count; ++i)
{
    var card = generator.Roll();
    Console.WriteLine("A {0} {1} {2} of the {3} {4}",
        card.Adverb != null ? card.Adverb.Name : "",
        card.CardClass.ToString(),
        card.Platonic.ToString(),
        card.Meta != null ? card.Meta.Name : "",
        card.Origin.ToString());
}
