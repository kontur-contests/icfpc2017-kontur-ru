using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using lib;

namespace ConsoleApp
{
    public class PlayerTournamentResult
    {
        public PlayerTournamentResult(AiFactory factory = null)
        {
            Factory = factory;
            Maps = new HashSet<string>();
        }

        public AiFactory Factory;
        public int GamesPlayed;
        public int ExceptionsCount;
        public StatValue GamesWon = new StatValue();
        public StatValue NormalizedMatchScores = new StatValue();
        public StatValue GainFuturesScoreRate = new StatValue();
        public StatValue GainFuturesCountRate = new StatValue();
        public HashSet<string> Maps;

        public StatValue TurnTime = new StatValue
        {
            ShowDispersion = true
        };

        public StatValue OptionUsageRate = new StatValue
        {
            ShowDispersion = true
        };

        public PlayerTournamentResult Clone()
        {
            return new PlayerTournamentResult(Factory)
            {
                ExceptionsCount = ExceptionsCount,
                GamesPlayed = GamesPlayed,
                GamesWon = GamesWon.Clone(),
                NormalizedMatchScores = NormalizedMatchScores.Clone(),
                GainFuturesCountRate = GainFuturesCountRate.Clone(),
                GainFuturesScoreRate = GainFuturesScoreRate.Clone(),
                TurnTime = TurnTime.Clone(),
                OptionUsageRate = OptionUsageRate.Clone()
            };
        }

        public static IEnumerable<PlayerTournamentResult> Merge(IEnumerable<PlayerTournamentResult> results)
        {
            return results.GroupBy(r => r.Factory.Name).Select(MergeGroup);
        }

        private static PlayerTournamentResult MergeGroup(IEnumerable<PlayerTournamentResult> results)
        {
            var r = new PlayerTournamentResult(null);
            foreach (var result in results)
            {
                r.Factory = result.Factory;
                r.GamesPlayed += result.GamesPlayed;
                r.ExceptionsCount += result.ExceptionsCount;
                r.GamesWon.AddAll(result.GamesWon);
                r.NormalizedMatchScores.AddAll(result.NormalizedMatchScores);
                r.GainFuturesScoreRate.AddAll(result.GainFuturesScoreRate);
                r.GainFuturesCountRate.AddAll(result.GainFuturesCountRate);
                foreach (var map in result.Maps)
                {
                    r.Maps.Add(map);
                }
            }
            return r;
        }
    }
}