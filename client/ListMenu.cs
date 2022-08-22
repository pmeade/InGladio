namespace client;

public class ListMenu
{
    public void Show(CardLibrary? cardLibrary)
    {
        int index = 0;
        Console.WriteLine();
        while (index < cardLibrary?.Cards?.Count)
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
}