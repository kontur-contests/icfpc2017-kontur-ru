using lib;

namespace ConsoleApp
{
    public class PlayerTournamentResult
    {
        public PlayerTournamentResult(AiFactory factory)
        {
            Factory = factory;
        }

        public AiFactory Factory;
        public int GamesPlayed;
        public int GamesWon;
        public int ExceptionsCount;
        public double WinRate => GamesWon / (double)GamesPlayed;
    }
}