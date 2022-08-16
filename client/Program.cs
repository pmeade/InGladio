// See https://aka.ms/new-console-template for more information

using client;

splash();
var name = login();
var mainMenu = new MainMenu();
mainMenu.Show(name);

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
