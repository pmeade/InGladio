namespace client;

public class StartMessage : Message
{
    public string? Name { get; private set; }
    
    public int Seed { get; private set; }
    public StartMessage(string? name, string? seed)
    {
        MessageType = EMessageType.Start;
        Name = name;
        Seed = Int32.Parse(seed ?? string.Empty);
    }

    protected override string? detailsToString()
    {
        return string.Format("{0},{1}", Name, Seed);
    }

    public static Message? FromNet(string?[] parts)
    {
        return new StartMessage(parts[1], parts[2]);
    }
}