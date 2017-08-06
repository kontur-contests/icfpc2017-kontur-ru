using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using lib.Ai.StrategicFizzBuzz;
using lib.GraphImpl;
using lib.Scores.Simple;
using lib.StateImpl;
using lib.Structures;
using lib.viz;
using NUnit.Framework;

namespace lib.Ai
{
    public class CrazyAi : IAi
    {
        public string Name => nameof(CrazyAi);
        public string Version => "0.1";
        
        private readonly Random random = new Random(314);
        
        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<GraphService>(state);
            services.Setup<MineDistCalculator>(state);
            return AiSetupDecision.Empty();
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            var graph = services.Get<GraphService>(state).Graph;

            var mines = state.map.Mines.ToList();
            for (int i = 0; i < mines.Count; i++)
            {
                for (int j = i + 1; j < mines.Count; j++)
                {
                    var denic = new Dinic(graph, state.punter, mines[i], mines[j], out int flow);
                    if (flow != 0 && flow != Dinic.INF)
                    {
                        var cut = denic.GetMinCut();
                        var edge = cut[random.Next(cut.Count)];
                        return AiMoveDecision.Claim(state.punter, edge.From, edge.To);
                    }
                }
            }

            var connectedCalculator = new ConnectedCalculator(graph, state.punter);
            GreedyAiHelper.TryExtendAnything(state.punter, graph, connectedCalculator, services.Get<MineDistCalculator>(state), out Move nextMove);
            return AiMoveDecision.Move(nextMove);
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