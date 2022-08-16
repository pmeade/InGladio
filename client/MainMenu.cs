namespace client;

public class MainMenu
{
    private CardMenu cardMenu = new CardMenu();
    private PlayMenu playMenu = new PlayMenu();
    public void Show(string name)
    {
        ConsoleKey choice = ConsoleKey.Clear;
        while (choice != ConsoleKey.Q)
        {
            choice = TextInput.Get(new Dictionary<ConsoleKey, string>()
                {
//                    { ConsoleKey.C, "Cards" },
                    { ConsoleKey.P, "Play" },
                    { ConsoleKey.Q, "Quit" },
                }
            );

            if (choice == ConsoleKey.C)
            {
                cardMenu.Show(name);
            }
            else if (choice == ConsoleKey.P)
            {
                playMenu.Show(name);
            }
        }
    }
}