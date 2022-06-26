﻿using System;
using System.Collections.Generic;

namespace lib
{
    public class Card
    {
        public Card()
        {
        }

        public Platonic Platonic { get; set; }
        
        public Power Power { get; set; }
        public Action<PlayerController> OnWinHand { get; set; }
        public Action<PlayerController> OnBurned { get; set; }
        public List<CardAttribute> CardAttributes { get; set; } = new List<CardAttribute>();
        public CardClass CardClass { get; set; }
        public Origin Origin { get; set; }
    }
}