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
            //if (TryExtendComponent(graph, out move))
            //    return move;
            if (TryBuildNewComponent(graph, out move))
                return move;

            // foreach myMines.adjastentRivers take best

            return new Pass();
        }

        private bool TryBuildNewComponent(Graph graph, out IMove move)
        {
            var queue = new Queue<QueueItem>();
            var used = new Dictionary<int, QueueItem>();
            foreach (var mineId in graph.Mines.Keys.Where(id => !myMines.Contains(id)))
            {
                var queueItem = new QueueItem
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
                    QueueItem prev;
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
                        var queueItem = new QueueItem
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

        private class QueueItem
        {
            public Vertex CurrentVertex;
            public Vertex SourceMine;
            public Edge FirstEdge;
        }
    }
}