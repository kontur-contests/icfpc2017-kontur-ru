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
        public double FuturesCompletedCount;
        public double FuturesCount;
        public double FuturesCompletementRate => FuturesCompletedCount / FuturesCount;
        public double NormalizedMatchScoresSum;
        public double AvNormalizedMatchScores => NormalizedMatchScoresSum / GamesPlayed;
        public double WinRate => GamesWon / (double)GamesPlayed;
    }
}