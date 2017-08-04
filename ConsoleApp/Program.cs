using System;
using System.Collections.Generic;
using System.Linq;
using lib;
using lib.Scores.Simple;
using lib.Strategies;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var maps = MapLoader.LoadDefaultMaps()
                .Where(m => m.Map.Mines.Length > 1)
                .OrderBy(m => m.Map.Rivers.Length).ToList();

            foreach (var map in maps)
            {
                var gamers = new List<IAi> { new GreedyAi(), new ConnectClosestMinesAi() };
                var gameSimulator = new GameSimulatorRunner(new SimpleScoreCalculator(), true);


                Console.WriteLine($"MAP: {map.Name}");
                var results = gameSimulator.SimulateGame(
                    gamers, map.Map);

                foreach (var gameSimulationResult in results)
                    Console.Write($"{gameSimulationResult.Gamer.Name} ");
                Console.WriteLine();
                foreach (var gameSimulationResult in results)
                    Console.Write($"{gameSimulationResult.Score} ");
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}