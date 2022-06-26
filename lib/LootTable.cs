using System;

namespace lib
{
    public class LootTable
    {
        private Random random = new Random(DateTime.Now.GetHashCode());
        private CardAttribute[] mundane = new CardAttribute[]{ new CardAttribute(){}};
        private CardAttribute[] higher = new CardAttribute[]{ new CardAttribute(){}};
        private CardAttribute[] epic = new CardAttribute[]{ new CardAttribute(){}};

        public Card Roll(int rolls)
        {
            var card = new Card()
            {
                Platonic = (Platonic)random.Next(2),
            };

            var powerRoll = random.Next(16);
            card.Power = rollToPower(powerRoll);

            while (rolls > 0)
            {
                rollAttribute(card);
                --rolls;
            }

            return card;
        }

        private void rollAttribute(Card card)
        {
            var roll = random.Next(16);
            if (roll < 8)
            {
                card.CardAttributes.Add(randomMundane());
            }
            else if (roll < 13)
            {
                card.CardAttributes.Add(randomHighPower());
            }
            else
            {
                card.CardAttributes.Add(randomEpicPower());
            }
        }

        private CardAttribute randomEpicPower()
        {
            return epic[random.Next() % mundane.Length];
        }

        private CardAttribute randomHighPower()
        {
            return higher[random.Next() % mundane.Length];
        }

        private CardAttribute randomMundane()
        {
            return mundane[random.Next() % mundane.Length];
        }

        private Power rollToPower(int powerRoll)
        {
            if (powerRoll < 8)
            {
                return Power.Three;
            }
            else if (powerRoll < 13)
            {
                return Power.Five;
            }

            return Power.Eight;
        }
    }
}