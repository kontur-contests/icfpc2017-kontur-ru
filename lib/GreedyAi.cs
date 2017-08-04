using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using lib.GraphImpl;
using lib.viz;
using NUnit.Framework;

namespace lib.Strategies
{
    public class GreedyAi : IAi
    {
        public string Name => nameof(GreedyAi);
        private int punterId;

        private MineDistCalculator mineDistCalulator;

        // ReSharper disable once ParameterHidesMember
        public void StartRound(int punterId, int puntersCount, Map map)
        {
            this.punterId = punterId;
            this.mineDistCalulator = new MineDistCalculator(new Graph(map));
        }

        public IMove GetNextMove(IMove[] prevMoves, Map map)
        {
            var graph = new Graph(map);

            if (TryExtendAnything(graph, out IMove nextMove))
                return nextMove;

            return new Pass();
        }

        private bool TryExtendAnything(Graph graph, out IMove nextMove)
        {
            var calculator = new ConnectedCalculator(graph, punterId);
            var maxAddScore = long.MinValue;
            Edge bestEdge = null;
            foreach (var vertex in graph.Vertexes.Values)
            {
                foreach (var edge in vertex.Edges.Where(x => x.Owner == -1))
                {
                    var fromMines = calculator.GetConnectedMines(edge.From);
                    var toMines = calculator.GetConnectedMines(edge.To);
                    long addScore;
                    if (fromMines.Count == 0)
                        addScore = Calc(toMines, edge.From);
                    else
                    {
                        if (toMines.Count != 0)
                            throw new InvalidOperationException("Attempt to connect two not empty components! WTF???");
                        addScore = Calc(fromMines, edge.To);
                    }
                    if (addScore > maxAddScore)
                    {
                        maxAddScore = addScore;
                        bestEdge = edge;
                    }
                }
            }
            if (bestEdge != null)
            {
                nextMove = new Move(bestEdge.From, bestEdge.To);
                Console.WriteLine($"TAKE {bestEdge.From} {bestEdge.To}");
                return true;
            }
            Console.WriteLine("PASS");
            nextMove = null;
            return false;
        }

        private long Calc(List<int> mineIds, int vertexId)
        {
            return mineIds.Sum(
                mineId =>
                {
                    var dist = mineDistCalulator.GetDist(mineId, vertexId);
                    return (long)dist * dist;
                });
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
            var map = MapLoader.LoadMap(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\maps\sample.json"));

            var ai = new GreedyAi();
            var simulator = new GameSimulator(map.Map);
            simulator.StartGame(new List<IAi> { ai });
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
}