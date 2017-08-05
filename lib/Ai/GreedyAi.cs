using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using lib.GraphImpl;
using lib.Scores.Simple;
using lib.viz;
using NUnit.Framework;

namespace lib.Ai
{
    public class GreedyAi : IAi
    {
        public string Name { get; set; } = nameof(GreedyAi);
        public int punterId { get; private set; }
        private GreedyAiHelper GreedyAiHelper;

        private MineDistCalculator mineDistCalulator;

        // ReSharper disable once ParameterHidesMember
        public Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            this.punterId = punterId;
            this.mineDistCalulator = new MineDistCalculator(new Graph(map));
            GreedyAiHelper = new GreedyAiHelper(punterId, mineDistCalulator);
            return new Future[0];
        }

        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            var graph = new Graph(map);

            GreedyAiHelper.TryExtendAnything(graph, out Move nextMove);
            return nextMove;
        }

        public string SerializeGameState()
        {
            throw new System.NotImplementedException();
        }

        public void DeserializeGameState(string gameState)
        {
            throw new System.NotImplementedException();
        }
    }

    [TestFixture]
    public class GreedyAi_Run
    {
        [Test]
        [STAThread]
        [Explicit]
        public void Show()
        {
            var form = new Form();
            var painter = new MapPainter();
            var map = MapLoader.LoadMap(
                Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\maps\sample.json"));

            var ai = new GreedyAi();
            var simulator = new GameSimulator(map.Map, new Settings());
            simulator.StartGame(new List<IAi> {ai});

            while (true)
            {
                var gameState = simulator.NextMove();
                painter.Map = gameState.CurrentMap;

                var panel = new ScaledViewPanel(painter)
                {
                    Dock = DockStyle.Fill
                };
                form.Controls.Add(panel);
                form.ShowDialog();
            }
        }

        [Test]
        public void Test1()
        {
            var gamers = new List<IAi> {new GreedyAi(), new GreedyAi()};
            var gameSimulator = new GameSimulatorRunner(new SimpleScoreCalculator());

            var results = gameSimulator.SimulateGame(
                gamers, MapLoader.LoadMapByName("gen1.json").Map, new Settings());

            foreach (var gameSimulationResult in results)
            {
                Console.Out.WriteLine(
                    "gameSimulationResult = {0}:{1}", gameSimulationResult.Gamer.Name, gameSimulationResult.Score);
            }
        }

        [Test]
        [Explicit]
        public void TestAllMaps()
        {
            var maps = MapLoader.LoadDefaultMaps().OrderBy(m => m.Map.Rivers.Length).ToList();

            foreach (var map in maps)
            {
                var gamers = new List<IAi> { new GreedyAi(),  };
                var gameSimulator = new GameSimulatorRunner(new SimpleScoreCalculator());
                

                Console.WriteLine($"MAP: {map.Name}");
                var results = gameSimulator.SimulateGame(
                    gamers, map.Map, new Settings());

                foreach (var gameSimulationResult in results)
                    Console.Write($"{gameSimulationResult.Gamer.Name} ");
                Console.WriteLine();
                foreach (var gameSimulationResult in results)
                    Console.Write($"{gameSimulationResult.Score} ");
                Console.WriteLine();
                Console.Out.Flush();
            }
        }
    }
}