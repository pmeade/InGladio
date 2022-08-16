using System.Text;

namespace client;

public class TextInput
{
    public static ConsoleKey Get(Dictionary<ConsoleKey, string> choices)
    {
        Console.WriteLine();
        StringBuilder sb = new StringBuilder();
        foreach (var option in choices)
        {
            sb.Append(string.Format("{0} ", displayChoice(option)));
        }

        Console.WriteLine(sb.ToString());
        
        bool inputAccepted = false;
        ConsoleKey choice = ConsoleKey.Clear;
        while (!inputAccepted)
        {
            var key = Console.ReadKey();
            if (choices.ContainsKey(key.Key))
            {
                choice = key.Key;
                inputAccepted = true;
            }
        }

        return choice;
    }

    private static string displayChoice(KeyValuePair<ConsoleKey, string> choice)
    {
        if (choice.Value.Contains(choice.Key.ToString()))
        {
            StringBuilder sb = new StringBuilder();
            int keyIndex = choice.Value.IndexOf(choice.Key.ToString());
            sb.Append(choice.Value.Substring(0, keyIndex));
            sb.Append(string.Format("[{0}]", choice.Key.ToString()));
            sb.Append(choice.Value.Substring(keyIndex + 1));
            return sb.ToString();
        }
        else
        {
            return string.Format("[{0}]{1}", choice.Key.ToString(), choice.Value);
        }
    }
}