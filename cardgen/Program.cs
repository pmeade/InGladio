// See https://aka.ms/new-console-template for more information

using System.Text;
using lib;

Console.WriteLine("cardgen");

var generator = Generator.FromSeed((int)(DateTime.Now.Ticks & 0x0000ffff));

var count = args.Length > 1 ? int.Parse(args[1]) : 64;

for (int i = 0; i < count; ++i)
{
    var card = generator.Roll(generator.Next() % ((i%8)+1));
    Console.WriteLine(i + " " + card.ToPrettyString());
}
