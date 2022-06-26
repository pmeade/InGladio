using lib;

namespace lib
{
    public class Origin
    {
        private static Origin[] classes = new Origin[]
        {
            new Origin() {Name = "Day"},
            new Origin() {Name = "Night"},
            new Origin() {Name = "Wood"},
            new Origin() {Name = "Traveller"},
            new Origin() {Name = "Comet"},
            new Origin() {Name = "Magician"},
            new Origin() {Name = "Fair"},
            new Origin() {Name = "End"}
        };
        
        public string Name { get; private set; }

        public static Origin Get(uint index)
        {
            return classes[index % classes.Length];
        }
    }
}