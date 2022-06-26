namespace lib
{
    public class CardClass
    {
        private static CardClass[] classes = new CardClass[]
        {
            new CardClass() {Name = "Restoring"},
            new CardClass() {Name = "Hefty"},
            new CardClass() {Name = "Sticky"},
            new CardClass() {Name = "Sharp"},
            new CardClass() {Name = "Eclipsed"},
            new CardClass() {Name = "Sturdy"},
            new CardClass() {Name = "Nimble"},
            new CardClass() {Name = "Pure"}
        };
        public string Name { get; private set; }

        public static CardClass Get(uint index)
        {
            return classes[index % classes.Length];
        }
    }
}