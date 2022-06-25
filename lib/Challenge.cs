using System.Text.RegularExpressions;

namespace lib
{
    public class Challenge
    {
        public bool Open { get; protected set; }
        public PlayerController Player { get; }
        public PlayerController Accepter { get; private set; }
        public SealedDeck Deck { get; private set; }

        private Challenge(PlayerController playerController)
        {
            this.Open = true;
            this.Player = playerController;
        }

        public Challenge()
        {
            this.Open = false;
        }

        public void Accept(PlayerController accepter)
        {
            if (Open)
            {
                Accepter = accepter;
                Open = false;
            }
        }

        public static Challenge Prebuilt(PlayerController playerController, SealedDeck deck)
        {
            return new Challenge(playerController)
            {
                Deck = deck
            };
        }

        public static Challenge Random(PlayerController playerController)
        {
            return new Challenge(playerController);
        }
    }
}