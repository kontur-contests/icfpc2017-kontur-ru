using System;
using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;

namespace lib
{
    public class ConnectClosestMinesAi : IAi
    {
        private HashSet<int> myMines = new HashSet<int>();
        
        private int punterId;
        private MineDistCalculator mineDistCalulator;

        public string Name => nameof(ConnectClosestMinesAi);

        // ReSharper disable once ParameterHidesMember
        public void StartRound(int punterId, int puntersCount, Map map)
        {
            this.punterId = punterId;
            this.mineDistCalulator = new MineDistCalculator(new Graph(map));
        }

        public IMove GetNextMove(IMove[] prevMoves, Map map)
        {
            var graph = new Graph(map);

            IMove move;

            if (TryExtendComponent(graph, out move))
                return move;

            if (TryBuildNewComponent(graph, out move))
                return move;

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
                return MakeMove(bestEdge);
            
            return new Pass();
        }

        private long Calc(List<int> mineIds, int vertexId)
        {
            return mineIds.Sum(
                mineId =>
                {
                    var dist = mineDistCalulator.GetDist(mineId, vertexId);
                    return (long) dist * dist;
                });
        }

        private bool TryExtendComponent(Graph graph, out IMove move)
        {
            var queue = new Queue<ExtendQueueItem>();
            var used = new HashSet<int>();
            foreach (var mineId in graph.Mines.Keys.Where(id => !myMines.Contains(id)))
            {
                var queueItem = new ExtendQueueItem
                {
                    CurrentVertex = graph.Vertexes[mineId],
                    Edge = null
                };
                queue.Enqueue(queueItem);
                used.Add(mineId);
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.CurrentVertex.Edges.Any(x => x.Owner == punterId))
                {
                    move = MakeMove(current.Edge);
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

        private bool TryBuildNewComponent(Graph graph, out IMove move)
        {
            var queue = new Queue<BuildQueueItem>();
            var used = new Dictionary<int, BuildQueueItem>();
            foreach (var mineId in graph.Mines.Keys.Where(id => !myMines.Contains(id)))
            {
                var queueItem = new BuildQueueItem
                {
                    CurrentVertex = graph.Vertexes[mineId],
                    SourceMine = graph.Vertexes[mineId],
                    FirstEdge = null
                };
                queue.Enqueue(queueItem);
                used.Add(mineId, queueItem);
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
                            myMines.Add(bestMine.Id);
                            if (bestMine == prev.SourceMine)
                            {
                                move = MakeMove(prev.FirstEdge ?? edge);
                                return true;
                            }
                            {
                                move = MakeMove(current.FirstEdge ?? edge);
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

        private static Vertex SelectBestMine(Vertex a, Vertex b)
        {
            return a.Edges.Count(x => x.Owner == -1) < b.Edges.Count(x => x.Owner == -1) ? a : b;
        }

        private static IMove MakeMove(Edge edge)
        {
            return new Move(edge.From, edge.To);
        }

        public string SerializeGameState()
        {
            throw new NotImplementedException();
        }

        public void DeserializeGameState(string gameState)
        {
            throw new NotImplementedException();
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
}