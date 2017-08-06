using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Structures;
using lib.viz;
using NUnit.Framework;

namespace lib.Ai
{
    public class ConnectClosestMinesAi : IAi
    {
        public string Name => nameof(ConnectClosestMinesAi);
        public string Version => "0.1";

        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<GraphService>(state);
            services.Setup<MineDistCalculator>(state);
            return AiSetupDecision.Empty();
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            AiMoveDecision move;
            if (TryExtendComponent(state, services, out move))
                return move;
            if (TryBuildNewComponent(state, services, out move))
                return move;
            if (TryExtendAnything(state, services, out move))
                return move;
            return AiMoveDecision.Pass(state.punter);
        }

        private bool TryExtendAnything(State state, IServices services, out AiMoveDecision nextMove)
        {
            var graph = services.Get<GraphService>(state).Graph;
            var mineDistCalculator = services.Get<MineDistCalculator>(state);
            var calculator = new ConnectedCalculator(graph, state.punter);
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
                        addScore = Calc(mineDistCalculator, toMines, edge.From);
                    else
                    {
                        if (toMines.Count != 0)
                        {
                            if (!toMines.SetEquals(fromMines))
                                throw new InvalidOperationException("Attempt to connect two not empty components! WTF???");
                            addScore = 0;
                        }
                        else
                            addScore = Calc(mineDistCalculator, fromMines, edge.To);
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
                nextMove = AiMoveDecision.Claim(state.punter, bestEdge.From, bestEdge.To);
                return true;
            }
            nextMove = null;
            return false;
        }

        private bool TryExtendComponent(State state, IServices services, out AiMoveDecision move)
        {
            //TODO ������ ����������� ������ ���������� ����������. � ����� ���� ����� ��������� ����� ������� ����������.
            var graph = services.Get<GraphService>(state).Graph;
            var queue = new Queue<ExtendQueueItem>();
            var used = new HashSet<int>();
            foreach (var mineV in GetNotMyMines(state, graph))
            {
                var queueItem = new ExtendQueueItem
                {
                    CurrentVertex = mineV,
                    Edge = null
                };
                queue.Enqueue(queueItem);
                used.Add(mineV.Id);
            }
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.CurrentVertex.Edges.Any(x => x.Owner == state.punter))
                {
                    if (current.Edge == null)
                        throw new InvalidOperationException("Mine is already part of component! WTF?");
                    move = AiMoveDecision.Claim(state.punter, current.Edge.From, current.Edge.To);
                    return true;
                }
                foreach (var edge in current.CurrentVertex.Edges.Where(x => x.Owner == -1))
                {
                    var next = graph.Vertexes[edge.To];
                    if (!used.Contains(next.Id))
                    {
                        var queueItem = new ExtendQueueItem
                        {
                            CurrentVertex = next,
                            Edge = edge
                        };
                        queue.Enqueue(queueItem);
                        used.Add(next.Id);
                    }
                }
            }
            move = null;
            return false;
        }

        private static IEnumerable<Vertex> GetNotMyMines(State state, Graph graph)
        {
            return graph.Mines.Values.Where(v => v.Edges.All(e => e.Owner != state.punter));
        }

        private bool TryBuildNewComponent(State state, IServices services, out AiMoveDecision move)
        {
            var graph = services.Get<GraphService>(state).Graph;
            var queue = new Queue<BuildQueueItem>();
            var used = new Dictionary<int, BuildQueueItem>();
            foreach (var mineV in GetNotMyMines(state, graph))
            {
                var queueItem = new BuildQueueItem
                {
                    CurrentVertex = mineV,
                    SourceMine = mineV,
                    FirstEdge = null
                };
                queue.Enqueue(queueItem);
                used.Add(mineV.Id, queueItem);
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var edge in current.CurrentVertex.Edges.Where(x => x.Owner == -1))
                {
                    var next = graph.Vertexes[edge.To];
                    BuildQueueItem prev;
                    if (used.TryGetValue(next.Id, out prev))
                    {
                        if (prev.SourceMine != current.SourceMine)
                        {
                            var bestMine = SelectBestMine(prev.SourceMine, current.SourceMine);
                            if (bestMine == prev.SourceMine)
                            {
                                Edge edge1 = prev.FirstEdge ?? edge;
                                move = AiMoveDecision.Claim(state.punter, edge1.From, edge1.To);
                                return true;
                            }
                            if (bestMine == current.SourceMine)
                            {
                                Edge edge1 = current.FirstEdge ?? edge;
                                move = AiMoveDecision.Claim(state.punter, edge1.From, edge1.To);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        var queueItem = new BuildQueueItem
                        {
                            CurrentVertex = next,
                            SourceMine = current.SourceMine,
                            FirstEdge = current.FirstEdge ?? edge
                        };
                        queue.Enqueue(queueItem);
                        used.Add(next.Id, queueItem);
                    }
                }
            }
            move = null;
            return false;
        }

        private long Calc(MineDistCalculator mineDistCalculator, HashSet<int> mineIds, int vertexId)
        {
            return mineIds.Sum(
                mineId =>
                {
                    var dist = mineDistCalculator.GetDist(mineId, vertexId);
                    return (long) dist * dist;
                });
        }

        private static Vertex SelectBestMine(Vertex a, Vertex b)
        {
            return a.Edges.Count(x => x.Owner == -1) < b.Edges.Count(x => x.Owner == -1) ? a : b;
        }

        private class BuildQueueItem
        {
            public Vertex CurrentVertex;
            public Vertex SourceMine;
            public Edge FirstEdge;
        }

        private class ExtendQueueItem
        {
            public Vertex CurrentVertex;
            public Edge Edge;
        }
    }

    [TestFixture]
    public class ConnectClosestMinesAi_Should
    {
        [Test]
        public void Test1()
        {
            var ai = new ConnectClosestMinesAi();
            var state = new State{punter = 0, punters = 1, map = MapLoader.LoadMap(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\maps\sample.json")).Map };
            var services = new Services();
            ai.Setup(state, services);
            var moveDecision = ai.GetNextMove(state, services);
            Assert.That(moveDecision.move, Is.EqualTo(Move.Claim(0, 5, 3)));
            state.map = state.map.ApplyMove(moveDecision.move);
            state.turns.Add(new TurnState());
            services.ApplyNextState(state);
            moveDecision = ai.GetNextMove(state, services);
            Assert.That(moveDecision.move, Is.EqualTo(Move.Claim(0, 1, 3)));
            state.map = state.map.ApplyMove(moveDecision.move);
            state.turns.Add(new TurnState());
            services.ApplyNextState(state);
            moveDecision = ai.GetNextMove(state, services);
            Assert.That(moveDecision.move, Is.EqualTo(Move.Claim(0, 0, 1)));
        }
        
        [Test]
        [STAThread]
        [Explicit]
        public void Show()
        {
            var form = new Form();
            var painter = new MapPainter();
            var map = MapLoader.LoadMap(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\maps\sample.json"));

            var ai = new ConnectClosestMinesAi();
            var simulator = new GameSimulator(map.Map, new Settings());
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