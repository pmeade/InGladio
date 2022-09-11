using System.Net;

namespace lib
{
    public class Play
    {
        public Card Card { get; private set;}
        public Choice Choice { get; private set; }
        
        public Target Target { get; private set;}
        
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

        public static Play Move(Card card, Place moveTo, Target opposedTarget, Mover mover, Place locationPlayedFrom)
        {
            if (card.Choice != Choice.Move)
            {
                throw new ProtocolViolationException();
            }
            return new MovePlay(card, moveTo, opposedTarget, mover, locationPlayedFrom);
        }

        private class MovePlay : Play
        {
            private readonly Mover mover;
            private Place location { get; }
            public MovePlay(Card card, Place location, Target target, Mover mover, Place locationPlayedFrom) : base(card, target, locationPlayedFrom)
            {
                this.location = location;
                this.mover = mover;
            }

            public override void Resolve(PlayerController player)
            {
                mover.UpdateLocation(location);
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
                throw new ProtocolViolationException();
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
                throw new ProtocolViolationException();
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

        public bool BeatsInMoveStrikeParry(Choice otherChoice)
        {
            return (Choice == Choice.Move && otherChoice == Choice.Parry)
                   || (Choice == Choice.Parry && otherChoice == Choice.Strike)
                   || (Choice == Choice.Strike && otherChoice == Choice.Move);
        }

        public bool BigEnough(Power otherEffectivePower)
        {
            return EffectivePower >= otherEffectivePower;
        }
    }
}