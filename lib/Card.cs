using System;
using System.Collections.Generic;
using System.Text;

namespace lib
{
    public class Card
    {
        public Card()
        {
        }

        public Choice Choice { get; set; }
        
        public Platonic Platonic { get; set; }
        
        public Power Power { get; set; }
        
        public Action<PlayerController> OnWinHand { get; set; }
        
        public Action<PlayerController> OnBurned { get; set; }
        public List<CardAttribute> CardAttributes { get; set; } = new List<CardAttribute>();
        public Adjective Adjective { get; set; }
        public Origin Origin { get; set; }
        public Adverb Adverb { get; set; }
        public Meta Meta { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0},", Adverb?.Name));
            sb.Append(string.Format("{0},", Adjective?.Name));
            sb.Append(string.Format("{0},", Platonic.ToString()));
            sb.Append(string.Format("{0},", Meta?.Name));
            sb.Append(string.Format("{0},", Origin?.Name));
            sb.Append(string.Format("{0},", Power.ToString()));
            sb.Append(string.Format("{0},", Choice.ToString()));

            return sb.ToString();
        }

        public string ToPrettyString()
        {
            StringBuilder sb = new StringBuilder();

            if (Burned)
            {
                sb.Append("***BURNED***");
            }
            
            
            sb.Append(string.Format("{0} --- {1} ---", Choice.ToString(), Power.ToString()));
            if (Adjective != null && Adjective.Name.Length != 0)
            {
                if (Adverb != null && Adverb.Name.Length != 0)
                {
                    sb.Append(string.Format(" {0}", Adverb.Name));
                }

                sb.Append(string.Format(" {0}", Adjective.Name));
            }

            sb.Append(string.Format(" {0}", Platonic.ToString()));

            if (Origin != null && Origin.Name.Length != 0)
            {
                sb.Append(" of the");
                if (Meta != null && Meta.Name.Length != 0)
                {
                    sb.Append(string.Format(" {0}", Meta.Name));
                }

                sb.Append(string.Format(" {0}", Origin.Name));
            }

            return sb.ToString();
        }

        public bool Burned { get; set; }

        public static Card FromString(string details)
        {
            var parts = details.Split(',');
            var adverb = new Adverb() { Name = parts[0] };
            var adjective = new Adjective() { Name = parts[1] };
            Platonic platonic;
            Platonic.TryParse(parts[2], true, out platonic);
            var meta = new Meta(){Name = parts[3]};
            var origin = new Origin(){Name = parts[4]};
            Power power;
            Power.TryParse(parts[5], true, out power);
            Choice choice;
            Choice.TryParse(parts[6], true, out choice);

            return new Card()
            {
                Adverb = adverb,
                Adjective = adjective,
                Platonic = platonic,
                Meta = meta,
                Origin = origin,
                Power = power,
                Choice = choice
            };
        }

        public static Card BasicMove()
        {
            return new Card() { Choice = Choice.Move };
        }

        public static Card BasicStrike()
        {
            return new Card() { Choice = Choice.Strike };
        }

        public static Card BasicParry()
        {
            return new Card() { Choice = Choice.Parry };
        }
    }
    
    namespace CardExtensionMethods
    {
        public static class CardExtensions
        { 
            public static Card AsRock(this Card card)
            {
                card.Platonic = Platonic.Rock;
                return card;
            }
            public static Card AsPaper(this Card card)
            {
                card.Platonic = Platonic.Paper;
                return card;
            }
            public static Card AsScissors(this Card card)
            {
                card.Platonic = Platonic.Scissors;
                return card;
            }
            public static Card AsThree(this Card card)
            {
                card.Power = Power.Three;
                return card;
            }

            public static Card AsFive(this Card card)
            {
                card.Power = Power.Five;
                return card;
            }

            public static Card AsEight(this Card card)
            {
                card.Power = Power.Eight;
                return card;
            }
        }
    }
}