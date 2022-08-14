namespace lib
{
    public class Game
    {
        public enum EGameState
        {
            Playing,
            Complete
        }

        public enum ERoundState
        {
            WaitingForPLays,
            WaitingForReveal,
            Resolved
        }
        
        public PlayerController Host;
        public PlayerController Challenger;
        public Basket Basket => Match.Basket;

        public Game Create(PlayerController host, PlayerController challenger)
        {
            var challenge = host.CreateChallenge();
            var match = challenger.AcceptChallenge(challenge);
            return new Game() { Host = host, Challenger = challenger, Match = match };
        }

        public Match Match { get; set; }
    }
}