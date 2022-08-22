// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
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

string? login()
{
    Console.Write("Name: ");
    string? name = null;
    while (string.IsNullOrEmpty(name))
    {
        name = Console.ReadLine();
    }

    return name;
}
