using System;
using System.Collections.Generic;
using System.Linq;

namespace lib
{
    public class Basket : Target
    {
        private readonly Generator generator;
        public Card Reward { get; private set; }
        
        public Card Face { get; private set; }
        public int Health { get; private set; } = 3;
        public Place Place { get; private set; } = Place.Square;
        public bool Open { get; private set; } = false;
        public PlayerController SnatchedBy { get; private set; }
        public List<PlayerController> damageDealers { get; } = new List<PlayerController>();

        public Basket(Generator generator)
        {
            this.generator = generator;
            RollFace();
        }

        public void RollFace()
        {
            Face = generator.Roll();
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

        public Place Location => Place;

        public void Claim(PlayerController playerController)
        {
            Open = true;
            SnatchedBy = playerController;
            this.Reward = this.generator.Roll(17);
        }
    }
}