namespace client;

public class PlayMenu
{
    public void Show(string name)
    {
        p2pclient client = new p2pclient();

        ConsoleKey choice = ConsoleKey.Clear;
        while (choice != ConsoleKey.Q)
        {
            choice = TextInput.Get(new Dictionary<ConsoleKey, string>()
                {
                    { ConsoleKey.H, "Host" },
                    { ConsoleKey.C, "Connect" },
                    { ConsoleKey.Q, "Quit" },
                }
            );

            if (choice == ConsoleKey.H)
            {
                client.Host(name);
            }
            else if (choice == ConsoleKey.C)
            {
                client.Connect(name);
            }
        }
    }
}