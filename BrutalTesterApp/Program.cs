using System;
using System.Collections.Generic;
using System.Linq;
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
        static Random random = new Random(122);
        static void Main(string[] args)
        {
            int minMapPlayersCount = 1;
            int maxMapPlayersCount = 8;
            int roundsCount = 10;

            var ais = new List<AiFactory>()
            {
                AiFactoryRegistry.CreateFactory<FutureIsNow>(),
                AiFactoryRegistry.CreateFactory<ConnectClosestMinesAi>(),
                //AiFactoryRegistry.CreateFactory<LochDinicKiller>(),
                AiFactoryRegistry.CreateFactory<LochMaxVertexWeighterKillerAi>(),
               // AiFactoryRegistry.CreateFactory<Podnaserator2000Ai>(),
                //AiFactoryRegistry.CreateFactory<LochKillerAi>(),
                AiFactoryRegistry.CreateFactory<GreedyAi>(),
            }
            .Select(f => new PlayerTournamentResult(f)).ToList();
            var maps = MapLoader.LoadOnlineMaps().Where(map => map.PlayersCount.InRange(minMapPlayersCount, maxMapPlayersCount)).ToList();

            for (int i = 0; i < roundsCount; i++)
                foreach (var map in maps)
                {
                    var matchPlayers = ais.Shuffle(random).Repeat().Take(map.PlayersCount).ToList();
                    var gameSimulator = new GameSimulatorRunner(new SimpleScoreCalculator(), true, false);
                    var gamers = matchPlayers.Select(p => p.Factory.Create()).ToList();
                    var results = gameSimulator.SimulateGame(gamers, map.Map, new Settings())
                        .OrderByDescending(r => r.Score).ToList();
                    int indexOfWinner = gamers.IndexOf(results[0].Gamer);
                    foreach (var res in results)
                    {
                        if (res.LastException != null)
                        {
                            int index = gamers.IndexOf(res.Gamer);
                            matchPlayers[index].ExceptionsCount++;
                        }

                    }
                    matchPlayers.ForEach(p => p.GamesPlayed++);
                    matchPlayers[indexOfWinner].GamesWon++;
                    ShowStatus(ais);
                }
        }

        private static void ShowStatus(List<PlayerTournamentResult> players)
        {
            Console.WriteLine();
            Console.WriteLine();
            var ordered = players.OrderBy(p => p.WinRate);
            var cols = new[] { 30, -7, -7, -7, -10 };
            FormatColumns(cols, "Name", "WinRate", "nPlayed", "nWon", "Exceptions");
            Console.WriteLine(new string('=', 70));
            foreach (var player in ordered)
            {
                FormatColumns(cols, player.Factory.Name, player.WinRate, player.GamesPlayed, player.GamesWon, player.ExceptionsCount > 0 ? player.ExceptionsCount.ToString() : "");
            }
        }

        private static void FormatColumns(int[] widths, params object[] values)
        {
            var res = "";
            foreach (var valueWithWidth in values.Zip(widths, (s, w) => (s.ToString(), w)))
            {
                int width = Math.Abs(valueWithWidth.Item2);
                int orientation = Math.Sign(valueWithWidth.Item2);
                string padded = orientation > 0 ? valueWithWidth.Item1.PadRight(width) : valueWithWidth.Item1.PadLeft(width);
                res += padded.Substring(0, Math.Min(width, padded.Length));
                res += " ";
            }
            Console.WriteLine(res);
        }
    }
}