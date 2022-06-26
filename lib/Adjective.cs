namespace lib
{
    public class Adjective
    {
        private static Adjective[] classes = new Adjective[]
        {
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            new Adjective() {Name = "Restoring"},
            new Adjective() {Name = "Hefty"},
            new Adjective() {Name = "Sticky"},
            new Adjective() {Name = "Sharp"},
            new Adjective() {Name = "Eclipsed"},
            new Adjective() {Name = "Sturdy"},
            new Adjective() {Name = "Nimble"},
            new Adjective() {Name = "Pure"}
        };
        public string Name { get; private set; }

        public static Adjective Get(uint index)
        {
            return classes[index % classes.Length];
        }
    }
}