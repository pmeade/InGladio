using System;
using System.Runtime.CompilerServices;

namespace lib
{
    public class Play
    {
        public Card Card { get; }
        public Choice Choice { get; set; }
        
        public Target Target { get; }
        
        public Power EffectivePower { get; private set; }

        public Play(Card card, Target target, Place locationPlayedFrom)
        {
            this.Card = card;
            this.Target = target;

            calculateEffectivePower(card, target, locationPlayedFrom);
        }

        private void calculateEffectivePower(Card card, Target target, Place locationPlayedFrom)
        {
            EffectivePower = card.Power;
            switch (locationPlayedFrom)
            {
                case Place.Steps:
                case Place.Curtain:
                    if (card.Choice == Choice.Strike && EffectivePower != Power.Three)
                        --EffectivePower;
                    break;
                case Place.Perch:
                    if (card.Choice == Choice.Strike && EffectivePower != Power.Eight)
                        ++EffectivePower;
                    break;
            }

            if (target.Location == Place.Curtain && card.Choice == Choice.Strike && EffectivePower != Power.Three)
            {
                --EffectivePower;
            }
        }

        public static Play Move(Card card, Place place, Target target, Place locationPlayedFrom)
        {
            if (card.Choice != Choice.Move)
            {
                return null;
            }
            return new MovePlay(card, place, target, locationPlayedFrom);
        }

        private class MovePlay : Play
        {
            private Place place { get; }
            public MovePlay(Card card, Place place, Target target, Place locationPlayedFrom) : base(card, target, locationPlayedFrom)
            {
                this.place = place;
            }

            public override void Resolve(PlayerController player)
            {
                Target.Move(place);
            }

            public override bool IsStoppedBy(Choice opposingChoice)
            {
                return opposingChoice == Choice.Strike;
            }
        }

        public virtual void Resolve(PlayerController player)
        {
        }

        internal static Play Parry(Card card, Target target, Place locationPlayedFrom)
        {
            if (card.Choice != Choice.Parry)
            {
                return null;
            }
            return new ParryPlay(card, target, locationPlayedFrom);
        }

        private class ParryPlay : Play
        {
            public ParryPlay(Card card, Target target, Place locationPlayedFrom) : base(card, target, locationPlayedFrom)
            {
                this.Choice = Choice.Parry;
            }
            public override void Resolve(PlayerController player)
            {
                if (Target != player)
                {
                    Target.TakeDamage(2, player);
                }
            }

            public override bool IsStoppedBy(Choice opposingChoice)
            {
                return (opposingChoice == Choice.Move || opposingChoice == Choice.Parry);
            }
        }

        public static Play Strike(Card card, Target target, Place locationPlayedFrom)
        {
            if (card.Choice != Choice.Strike)
            {
                return null;
            }
            return new StrikePlay(card, target, locationPlayedFrom);
        }

        private class StrikePlay : Play
        {
            public StrikePlay(Card card, Target target, Place locationPlayedFrom) : base(card, target, locationPlayedFrom)
            {
                this.Choice = Choice.Strike;
            }
            
            public override void Resolve(PlayerController player)
            {
                if (Target != player)
                {
                    Target.TakeDamage(1, player);
                }
            }

            public override bool IsStoppedBy(Choice opposingChoice)
            {
                return opposingChoice == Choice.Parry;
            }
        }

        public virtual bool IsStoppedBy(Choice opposingChoice)
        {
            return true;
        }

        public bool BeatsInMoveStrikeParry(Play otherPlay)
        {
            return (Choice == Choice.Move && otherPlay.Choice == Choice.Parry)
                   || (Choice == Choice.Parry && otherPlay.Choice == Choice.Strike)
                   || (Choice == Choice.Strike && otherPlay.Choice == Choice.Move);
        }

        public bool BigEnough(Play otherPlay)
        {
            return EffectivePower >= otherPlay.EffectivePower;
        }
    }
}