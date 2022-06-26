using System;
using System.Runtime.CompilerServices;

namespace lib
{
    public class Play
    {
        public Card Card { get; }
        public Choice Choice { get; set; }
        
        public Target Target { get; }

        public Play(Card card, Target target)
        {
            this.Card = card;
            this.Target = target;
        }

        public static Play Move(Card card, Place place, Target target)
        {
            return new MovePlay(card, place, target);
        }

        private class MovePlay : Play
        {
            private Place place { get; }
            public MovePlay(Card card, Place place, Target target) : base(card, target)
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

        internal static Play Parry(Card card, Target target)
        {
            return new ParryPlay(card, target);
        }

        private class ParryPlay : Play
        {
            public ParryPlay(Card card, Target target) : base(card, target)
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

        public static Play Strike(Card card, Target target)
        {
            return new StrikePlay(card, target);
        }

        private class StrikePlay : Play
        {
            public StrikePlay(Card card, Target target) : base(card, target)
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

        public bool Wins(Play otherPlay)
        {
            return (Choice == Choice.Move && otherPlay.Choice == Choice.Parry)
                   || (Choice == Choice.Parry && otherPlay.Choice == Choice.Strike)
                   || (Choice == Choice.Strike && otherPlay.Choice == Choice.Move);
        }

        public bool BigEnough(Play otherPlay)
        {
            return Card.Power >= otherPlay.Card.Power;
        }
    }
}