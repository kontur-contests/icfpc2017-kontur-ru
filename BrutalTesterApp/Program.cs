using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using ConsoleApp;
using lib;
using lib.Ai;
using lib.Ai.StrategicFizzBuzz;
using lib.Scores.Simple;
using lib.Structures;
using lib.viz;
using MoreLinq;

namespace BrutalTesterApp
{
    class Program
    {
        static Random random = new Random();
        private static DateTime lastUpdate = DateTime.MinValue;

        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            int minMapPlayersCount = 1;
            int maxMapPlayersCount = 4;
            int roundsCount = 100000;
            bool failOnExceptions = false;

            //var ais = AiFactoryRegistry.Factories
            var ais = new List<AiFactory>()
            {
                AiFactoryRegistry.CreateFactory<FutureIsNow>(),
            //    AiFactoryRegistry.CreateFactory<ConnectClosestMinesAi>(),
                AiFactoryRegistry.CreateFactory<LochDinicKillerAi>(),
                //AiFactoryRegistry.CreateFactory<LochMaxVertexWeighterKillerAi>(),
                AiFactoryRegistry.CreateFactory<AllComponentsMaxReachableVertexWeightAi>(),
                //AiFactoryRegistry.CreateFactory<MaxReachableVertexWeightAi>(),
                //AiFactoryRegistry.CreateFactory<Podnaserator2000Ai>(),
                AiFactoryRegistry.CreateFactory<ConnectClosestMinesAi>(),
                AiFactoryRegistry.CreateFactory<GreedyAi>(),
                AiFactoryRegistry.CreateFactory<RandomEWAi>(),

                
            //    AiFactoryRegistry.CreateFactory<Podnaserator2000Ai>(),
            //    AiFactoryRegistry.CreateFactory<LochKillerAi>(),
            //    AiFactoryRegistry.CreateFactory<GreedyAi>(),
            //    AiFactoryRegistry.CreateFactory<GreedierAi>(),
            //    AiFactoryRegistry.CreateFactory<AgileMaxVertexWeighterAi>(),
            //    AiFactoryRegistry.CreateFactory<AgileMaxVertexWeighterAi>(),
            }
            .Select(f => new PlayerTournamentResult(f)).ToList();
            var maps = MapLoader.LoadOnlineMaps().Where(map => map.PlayersCount.InRange(minMapPlayersCount, maxMapPlayersCount)).ToList();

            for (int i = 0; i < roundsCount; i++)
            {
                foreach (var map in maps)
                {
                    var matchPlayers = ais.Shuffle(random).Repeat().Take(map.PlayersCount).ToList();
                    var gameSimulator = new GameSimulatorRunner(new SimpleScoreCalculator(), true, !failOnExceptions);
                    var gamers = matchPlayers.Select(p => p.Factory.Create()).ToList();
                    var results = gameSimulator.SimulateGame(gamers, map.Map, new Settings(true, true));
                    AssignMatchScores(results);
                    foreach (var res in results)
                    {
                        int index = gamers.IndexOf(res.Gamer);
                        var player = matchPlayers[index];
                        player.GamesPlayed++;
                        player.NormalizedMatchScores.Add((double)res.MatchScore / matchPlayers.Count);
                        player.GamesWon.Add(res.MatchScore == matchPlayers.Count ? 1 : 0);
                        if (res.LastException != null)
                            player.ExceptionsCount++;
                        if (res.ScoreData.PossibleFuturesScore != 0)
                            player.GainFuturesScoreRate.Add((double)res.ScoreData.GainedFuturesScore / res.ScoreData.PossibleFuturesScore);
                        if (res.ScoreData.TotalFuturesCount != 0)
                            player.GainFuturesCountRate.Add((double)res.ScoreData.GainedFuturesCount / res.ScoreData.TotalFuturesCount);
                    }
                    ShowStatus(ais, maps);
                }
            }
        }

        private static void AssignMatchScores(List<GameSimulationResult> results)
        {
            results = results.OrderByDescending(r => r.Score).ToList();
            var score = results.Count;
            results[0].MatchScore = score;
            for (int i = 1; i < results.Count; i++)
            {
                if (results[i].MatchScore < results[i - 1].MatchScore) score = results.Count - i;
                results[i].MatchScore = score;
            }
        }

        private static void ShowStatus(List<PlayerTournamentResult> players, List<NamedMap> maps)
        {
            if (DateTime.Now < lastUpdate + TimeSpan.FromMilliseconds(500)) return;
            lastUpdate = DateTime.Now;

            Console.Clear();
            Console.WriteLine("Maps: " + maps.Select(m => m.Name).ToDelimitedString(", "));
            Console.WriteLine();
            var ordered = players.OrderByDescending(p => p.NormalizedMatchScores.Mean);
            var cols = new[] { 20, -14, -14, -7, -14, -14, -14 };
            FormatColumns(cols, "Name", "WinRate", "NMS", "N", "FNCount", "FNScore", "Exceptions");
            Console.WriteLine(new string('=', 100));
            foreach (var player in ordered)
            {
                FormatColumns(cols,
                    player.Factory.Name,
                    player.GamesWon,
                    player.NormalizedMatchScores,
                    player.GamesPlayed,
                    player.GainFuturesCountRate,
                    player.GainFuturesScoreRate,
                    player.ExceptionsCount > 0 ? player.ExceptionsCount.ToString() : "");
            }
        }

        private static void FormatColumns(int[] widths, params object[] values)
        {
            var res = "";
            foreach (var valueWithWidth in values.Zip(widths, (s, w) => (FormatValue(s), w)))
            {
                int width = Math.Abs(valueWithWidth.Item2);
                int orientation = Math.Sign(valueWithWidth.Item2);
                string padded = orientation > 0 ? valueWithWidth.Item1.PadRight(width) : valueWithWidth.Item1.PadLeft(width);
                res += padded.Substring(0, Math.Min(width, padded.Length));
                res += " ";
            }
            Console.WriteLine(res);
        }

        private static string FormatValue(object o)
        {
            if (o is double d) return double.IsNaN(d) ? "" : d.ToString("0.00");
            if (o is StatValue st) return st.Count == 0 ? "" : (st.Mean.ToString("0.00") + " +-" + st.ConfIntervalSize.ToString(".00"));
            return o.ToString();
        }
    }
}