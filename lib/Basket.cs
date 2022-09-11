using System;
using System.Collections.Generic;
using System.Linq;

namespace lib
{
    public class Basket : Target, Mover
    {
        private readonly Generator generator;

        private Target target;
        
        private readonly Func<Place, Target> targetFinder;
        public Card Reward { get; private set; }
        
        public Card Face { get; private set; }
        public int Health { get; private set; } = 3;
        public Place Place { get; private set; } = Place.Square;
        public bool Open { get; private set; } = false;
        public PlayerController SnatchedBy { get; private set; }
        public List<PlayerController> damageDealers { get; } = new List<PlayerController>();

        public Basket(Generator generator, Func<Place, Target> targetFinder)
        {
            this.generator = generator;
            this.targetFinder = targetFinder;
            RollFace();
        }

        public void RollFace()
        {
            Face = generator.Roll();
            _updateTarget();
            if (target != null)
            {
                _makePlay();
            }
        }

        private void _makePlay()
        {
            switch (Face.Choice)
            {
                case Choice.Move:
                    ActivePlay = Play.Move(Face, Location, target, this, Location);
                    break;
                case Choice.Parry:
                    ActivePlay = Play.Parry(Face, target, Location);
                    break;
                case Choice.Strike:
                    ActivePlay = Play.Strike(Face, target, Location);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void _updateTarget()
        {
            if (target?.Location != Location)
            {
                target = targetFinder.Invoke(Location);
            }
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

        public void UpdateLocation(Place place)
        {
            Place = place;
        }

        public Place Location => Place;
        public Play ActivePlay { get; set; }

        public void Claim(PlayerController playerController)
        {
            Open = true;
            SnatchedBy = playerController;
            this.Reward = this.generator.Roll(17);
        }
    }
}