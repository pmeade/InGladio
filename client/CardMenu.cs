namespace client;

public class CardMenu
{
    private ListMenu listMenu = new ListMenu();
    private DeckBuilder deckBuilder = new DeckBuilder();
    public void Show(string? name)
    {
        var fileName = $"{name}.cards";
        var library = CardLibrary.Load(fileName);
        var choice = ConsoleKey.L;

        while (choice != ConsoleKey.M)
        {
            Console.WriteLine();
            Console.WriteLine($"You have {library?.Cards?.Count} cards in your library");
            choice = TextInput.Get(new Dictionary<ConsoleKey, string>()
                {
                    { ConsoleKey.G, "Generate new cards" },
                    { ConsoleKey.L, "List all cards" },
                    { ConsoleKey.D, "Deck Builder"},
                    { ConsoleKey.M, "Main Menu" },
                }
            );

            if (choice == ConsoleKey.G)
            {
                library?.Generate(16);
                library?.Save(fileName);
            }

            if (choice == ConsoleKey.L)
            {
                listMenu.Show(library);
            }

            if (choice == ConsoleKey.D)
            {
                deckBuilder.Show(library);
            }
        }
    }

}