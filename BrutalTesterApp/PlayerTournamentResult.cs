using System.Collections.Generic;
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
        public int ExceptionsCount;
        public StatValue GamesWon = new StatValue();
        public StatValue NormalizedMatchScores = new StatValue();
        public StatValue GainFuturesScoreRate = new StatValue();
        public StatValue GainFuturesCountRate = new StatValue();
        public StatValue TurnTime = new StatValue
        {
            ShowDispersion = true
        };
    }
}