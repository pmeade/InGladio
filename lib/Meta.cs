﻿namespace lib
{
    public class Meta
    {
        private static Meta[] metas = new Meta[]
        {
            new Meta() {Name = "Hesitating"},
            new Meta() {Name = "Unkind"},
            new Meta() {Name = "Astonishing"},
            new Meta() {Name = "Restless"},
            new Meta() {Name = "Heroic"},
            new Meta() {Name = "Arcane"},
            new Meta() {Name = "Everlasting"},
            new Meta() {Name = "Sublime"}
        };
        public string Name { get; set; }

        public static Meta Get(uint index)
        {
            return metas[index % metas.Length];
        }
    }
}