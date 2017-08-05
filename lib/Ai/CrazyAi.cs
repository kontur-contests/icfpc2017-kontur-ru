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
    public class CrazyAi : IAi
    {
        public string Name => nameof(CrazyAi);
        private int punterId;
        private readonly Random random = new Random(314);
        private MineDistCalculator mineDistCalulator;
        private GreedyAiHelper GreedyAiHelper { get; set; }

        // ReSharper disable once ParameterHidesMember
        public Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            this.punterId = punterId;
            this.mineDistCalulator = new MineDistCalculator(new Graph(map));
            this.GreedyAiHelper = new GreedyAiHelper(punterId, mineDistCalulator);

            return new Future[0];
        }

        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            var graph = new Graph(map);

            var mines = map.Mines.ToList();
            for (int i = 0; i < mines.Count(); i++)
            {
                for (int j = i + 1; j < mines.Count(); j++)
                {
                    var denic = new Dinic(graph, punterId, mines[i], mines[j], out int flow);
                    if (flow != 0 && flow != Dinic.INF)
                    {
                        var cut = denic.GetMinCut();
                        var edge = cut[random.Next(cut.Count)];
                        return new ClaimMove(punterId, edge.From, edge.To);
                    }
                }
            }

            GreedyAiHelper.TryExtendAnything(graph, out Move nextMove);
            return nextMove;
        }

        public string SerializeGameState()
        {
            return "";
        }

        public void DeserializeGameState(string gameState)
        {
        }
    }

    [TestFixture]
    public class CrazyAi_Run
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
            simulator.StartGame(new List<IAi> { ai });

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
            var gamers = new List<IAi> { new CrazyAi(), new ConnectClosestMinesAi() };
            var gameSimulator = new GameSimulatorRunner(new SimpleScoreCalculator());

            var results = gameSimulator.SimulateGame(
                gamers, MapLoader.LoadMapByName("edge.json").Map, new Settings());

            foreach (var gameSimulationResult in results)
            {
                Console.Out.WriteLine(
                    "gameSimulationResult = {0}:{1}", gameSimulationResult.Gamer.Name, gameSimulationResult.Score);
            }
        }
    }
}