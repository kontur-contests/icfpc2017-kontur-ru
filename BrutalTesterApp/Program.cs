using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp;
using lib;
using lib.Ai;
using lib.Ai.StrategicFizzBuzz;
using lib.Scores.Simple;
using lib.Structures;
using lib.viz;
using MoreLinq;
using Newtonsoft.Json;

namespace BrutalTesterApp
{
    class Program
    {
        static Random random = new Random();
        private static DateTime lastUpdate = DateTime.MinValue;

        static void Main(string[] args)
        {
            if (args[0] == @"\merge")
            {
                if (args.Length == 1)
                    Merge(Environment.CurrentDirectory);
                else
                    Merge(args[1]);
                return;
            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            int minMapPlayersCount = 2;
            int maxMapPlayersCount = int.Parse(args[0]);
            int roundsCount = int.Parse(args[1]);
            bool failOnExceptions = true;

            //var ais = AiFactoryRegistry.ForOnlineRunsFactories
            var ais = new List<AiFactory>()
            {
                AiFactoryRegistry.CreateFactory<OptAntiLochDinicKillerAi>(),
                //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_0>(),
                //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_005>(),
                //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_01>(),
                //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_02>(),
                //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_03>(),
                //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_04>(),
                //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_05>(),
                AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_1>(),
                AiFactoryRegistry.CreateFactory<FutureIsNowAi>(),
                AiFactoryRegistry.CreateFactory<ConnectClosestMinesAi>(),
                //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi>(),
                AiFactoryRegistry.CreateFactory<LochDinicKillerAi>(),
                AiFactoryRegistry.CreateFactory<LochMaxVertexWeighterKillerAi>(),
                //AiFactoryRegistry.CreateFactory<AllComponentsMaxReachableVertexWeightAi>(),
                //AiFactoryRegistry.CreateFactory<MaxReachableVertexWeightAi>(),
                //AiFactoryRegistry.CreateFactory<ConnectClosestMinesAi>(),
                AiFactoryRegistry.CreateFactory<GreedyAi>(),
                AiFactoryRegistry.CreateFactory<RandomEWAi>(),
                //AiFactoryRegistry.CreateFactory<TheUberfullessnessAi>(),
            }
            .Select(f => new PlayerTournamentResult(f)).ToList();
            var maps = MapLoader.LoadOnlineMaps()
                .Where(map => map.PlayersCount.InRange(minMapPlayersCount, maxMapPlayersCount))
                //.Where(map => map.Name == "boston-sparse")
				.Where(map => args.Contains(map.Name))
                .ToList();

            var r = Enumerable.Range(0, roundsCount)
                .AsParallel()
                .Select(i => 
                {
                    return maps
                        .AsParallel()
                        .Select(map => 
                        {
                            var matchPlayers = ais.Select(a => a.Clone()).Shuffle(random).Repeat().Take(map.PlayersCount).ToList();
                            var gameSimulator = new GameSimulatorRunner(new SimpleScoreCalculator(), true, !failOnExceptions);
                            var gamers = matchPlayers.Select(p => p.Factory.Create()).ToList();
                            var results = gameSimulator.SimulateGame(gamers, map.Map, new Settings(true, true, true));
                            AssignMatchScores(results);
                            foreach (var res in results)
                            {
                                int index = gamers.IndexOf(res.Gamer);
                                var player = matchPlayers[index];
                                player.Maps.Add(map.Name);
                                player.GamesPlayed++;
                                player.OptionUsageRate.Add(res.OptionsUsed);
                                player.NormalizedMatchScores.Add((double)res.MatchScore / matchPlayers.Count);
                                player.GamesWon.Add(res.MatchScore == matchPlayers.Count ? 1 : 0);
                                if (res.LastException != null)
                                    player.ExceptionsCount++;
                                if (res.ScoreData.PossibleFuturesScore != 0)
                                    player.GainFuturesScoreRate.Add((double)res.ScoreData.GainedFuturesScore / res.ScoreData.PossibleFuturesScore);
                                if (res.ScoreData.TotalFuturesCount != 0)
                                    player.GainFuturesCountRate.Add((double)res.ScoreData.GainedFuturesCount / res.ScoreData.TotalFuturesCount);
                                player.TurnTime.AddAll(res.TurnTime);
                            }
                            return matchPlayers;
                        })
                        .Aggregate(new List<PlayerTournamentResult>(), (l, d) => { l.AddRange(d); return l;});
                })
                .Aggregate(new List<PlayerTournamentResult>(), (l, d) => { l.AddRange(d); return l; });

            r = PlayerTournamentResult.Merge(r).ToList();
            ShowStatus(r, maps.Select(m => m.Name).ToList());
            File.WriteAllText($"{Guid.NewGuid()}.json", JsonConvert.SerializeObject(r), Encoding.UTF8);
        }

        private static void Merge(string dir)
        {
            var res = new List<PlayerTournamentResult>();
            foreach (var file in Directory.GetFiles(dir).Where(f => f.EndsWith(".json") && !f.EndsWith(".merge.json")))
            {
                var list = JsonConvert.DeserializeObject<List<PlayerTournamentResult>>(File.ReadAllText(file, Encoding.UTF8));
                res.AddRange(list);
            }
            res = PlayerTournamentResult.Merge(res).ToList();
            ShowStatus(res, res.SelectMany(r => r.Maps).ToList());
            File.WriteAllText($"{Guid.NewGuid()}.merge.json", JsonConvert.SerializeObject(res), Encoding.UTF8);
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

        private static void ShowStatus(List<PlayerTournamentResult> players, IList<string> maps)
        {
            if (DateTime.Now < lastUpdate + TimeSpan.FromMilliseconds(500)) return;
            lastUpdate = DateTime.Now;

            Console.Clear();
            Console.WriteLine("Maps: " + maps.Distinct().ToDelimitedString(", "));
            Console.WriteLine();
            var ordered = players.OrderByDescending(p => p.NormalizedMatchScores.Mean);
            var cols = new[] { 25, -13, -13, -7, -13, -13, -13, -13, -13 };
            FormatColumns(cols, "Name", "WinRate", "NMS", "N", "OptUsed", "FNScore", "Turn, ms", "Exceptions");
            Console.WriteLine(new string('=', 120));
            foreach (var player in ordered)
            {
                FormatColumns(cols,
                    player.Factory.Name,
                    player.GamesWon,
                    player.NormalizedMatchScores,
                    player.GamesPlayed,
                    player.OptionUsageRate,
                    player.GainFuturesScoreRate,
                    player.TurnTime,
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