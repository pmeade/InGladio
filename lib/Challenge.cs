using System;
using System.Text.RegularExpressions;

namespace lib
{
    public class Challenge
    {
        public bool Open { get; protected set; }
        public PlayerController Host { get; }
        public PlayerController challenger { get; private set; }
        public event EventHandler ChallengeAccepted;

        private Challenge(PlayerController playerController)
        {
            this.Open = true;
            this.Host = playerController;
        }

        public Challenge()
        {
            this.Open = false;
        }

        public void Accept(PlayerController accepter)
        {
            if (Open)
            {
                challenger = accepter;
                Open = false;
                ChallengeAccepted?.Invoke(this, EventArgs.Empty);
            }
        }

        public static Challenge Prebuilt(PlayerController playerController, SealedDeck deck)
        {
            playerController.Deck = deck;
            return new Challenge(playerController);
        }

        public static Challenge Random(PlayerController playerController)
        {
            return new Challenge(playerController);
        }
    }
}