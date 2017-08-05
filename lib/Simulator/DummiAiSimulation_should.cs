using System;
using System.Collections.Generic;
using lib.Ai;
using lib.Scores.Simple;
using lib.Structures;
using NUnit.Framework;

namespace lib
{
    [TestFixture]
    public class DummiAiSimulation_should
    {
        [Test]
        public void Test1()
        {
            var gamers = new List<IAi> {new DummyAi(0.5), new DummyAi(0.5)};
            var gameSimulator = new GameSimulatorRunner(new SimpleScoreCalculator());

            var results = gameSimulator.SimulateGame(
                gamers, MapLoader.LoadMapByName("sample.json").Map, new Settings());

            foreach (var gameSimulationResult in results)
            {
                Console.Out.WriteLine("gameSimulationResult = {0}:{1}", gameSimulationResult.Gamer.Name, gameSimulationResult.Score);
            }
        }
    }
}