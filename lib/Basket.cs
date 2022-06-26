using System;
using System.Collections.Generic;
using System.Linq;

namespace lib
{
    public class Basket : Target
    {
        private readonly LootTable lootTable;
        public Card Card { get; private set; }
        public int Health { get; private set; } = 3;
        public Place Place { get; private set; } = Place.Square;
        public bool Open { get; private set; } = false;
        public PlayerController SnatchedBy { get; private set; }
        public List<PlayerController> damageDealers { get; } = new List<PlayerController>();

        public Basket(LootTable lootTable)
        {
            this.lootTable = lootTable;
        }

        public void TakeDamage(int amount, PlayerController dealer)
        {
            Health = Math.Max(Health - amount, 0);
            damageDealers.Add(dealer);
        }

        public void CheckForVictory()
        {
            if (Health == 0)
            {
                if (damageDealers.Count == 1)
                {
                    Claim(damageDealers.First());
                }
            }
        }

        public void Move(Place place)
        {
            Place = place;
        }

        public void Claim(PlayerController playerController)
        {
            Open = true;
            SnatchedBy = playerController;
            this.Card = lootTable.Roll(17);
        }
    }
}