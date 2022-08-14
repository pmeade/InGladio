using lib;
namespace client;

public class Player
{
    public PlayerController controller;
    public string Name;

    public Player(string name)
    {
        Name = name;
        controller = new PlayerController();
        
    }
}