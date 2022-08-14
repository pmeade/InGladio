// See https://aka.ms/new-console-template for more information

using client;
using lib;

p2pclient client = new p2pclient();

splash();

var name = login();

mainMenu(name);

void splash()
{
    var splashScreen =
        "**********************************************\n" +
        "**********************************************\n" +
        "**                                          **\n" +
        "**                                          **\n" +
        "**               In Gladio                  **\n" +
        "**                                          **\n" +
        "**                                          **\n" +
        "**********************************************\n" +
        "";
    Console.Write(splashScreen);
}

string login()
{
    Console.Write("Name: ");
    var name = "";
    while (name.Length == 0)
    {
        name = Console.ReadLine();
    }

    return name;
}

void mainMenu(string name)
{
    var choice = TextInput.Get(new Dictionary<ConsoleKey, string>()
        {
            { ConsoleKey.C, "Cards" },
            { ConsoleKey.P, "Play" },
        }
    );

    if (choice == ConsoleKey.C)
    {
        cardMenu(name);
    }
    else
    {
        playMenu();
    }
}

void cardMenu(string name)
{
    var fileName = string.Format("{0}.cards", name);
    var library = CardLibrary.Load(fileName);
    var choice = ConsoleKey.L;

    while (choice != ConsoleKey.M)
    {
        Console.WriteLine("You have {0} cards in your library", library.Cards.Count);
        choice = TextInput.Get(new Dictionary<ConsoleKey, string>()
            {
                { ConsoleKey.G, "Generate" },
                { ConsoleKey.L, "List" },
                { ConsoleKey.M, "Main Menu" },
            }
        );

        if (choice == ConsoleKey.G)
        {
            library.Generate(16);
            library.Save(fileName);
        }

        if (choice == ConsoleKey.L)
        {
            listMenu(library);

        }
    }
}

void listMenu(CardLibrary cardLibrary)
{
    int index = 0;
    Console.WriteLine();
    while (index < cardLibrary.Cards.Count)
    {
        for (int i = 0; i < 16 && i < cardLibrary.Cards.Count; ++i)
        {
            Console.WriteLine(cardLibrary.Cards[i + index].ToPrettyString());
        }

        index += 16;

        if (index < cardLibrary.Cards.Count)
        {
            Console.WriteLine("--(more)--");
            Console.ReadKey();
            Console.WriteLine();
        }
    }
}

void playMenu()
{
    
}

void playGame()
{
    Console.WriteLine("[H]ost [C]onnect");
    var key = Console.ReadKey();

    if (key.Key == ConsoleKey.H)
    {
        client.Host();
    }
    else if (key.Key == ConsoleKey.C)
    {
        client.Connect();
    }
}
