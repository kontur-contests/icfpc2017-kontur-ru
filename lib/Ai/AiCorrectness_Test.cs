using System;
using System.Collections.Generic;
using System.Linq;
using lib.Scores.Simple;
using lib.Structures;
using lib.viz;
using NUnit.Framework;

namespace lib.Ai
{
    [TestFixture("gen1.json", 2)]
    [TestFixture("circle.json", 2)]
    [TestFixture("lambda.json", 2)]
    [TestFixture("randomMedium.json", 4)]
    public class AiCorrectness_Test
    {
        private string MapName { get; }
        private int Players { get; }

        public AiCorrectness_Test(string mapName, int players)
        {
            MapName = mapName;
            Players = players;
        }

        private static readonly string[] Exceptions =
        {
//            nameof(MeetInTheMiddleAi)
        };

        private static IEnumerable<TestCaseData> TestCases => AiFactoryRegistry.ForOnlineRunsFactories
            .Where(factory => !Exceptions.Contains(factory.Name))
            .Select(factory => new TestCaseData(factory).SetName(factory.Name));

        [TestCaseSource(nameof(TestCases))]
        public void Check(AiFactory factory)
        {
            var gamers = new List<IAi> {factory.Create(), factory.Create()};
            var gameSimulator = new GameSimulatorRunner(new SimpleScoreCalculator());

            var results = gameSimulator.SimulateGame(
                gamers, MapLoader.LoadMapByName(MapName).Map, new Settings());

            foreach (var gameSimulationResult in results)
                Console.Out.WriteLine(
                    "gameSimulationResult = {0}:{1}", gameSimulationResult.Gamer.Name, gameSimulationResult.Score);
        }
    }
}