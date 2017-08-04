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

        public string Name => nameof(ConnectClosestMinesAi);

        public void StartRound(int punterId, int puntersCount, Map map)
        {
            this.punterId = punterId;
            // precalc (site,mine) -> score
        }

        public IMove GetNextMove(IMove[] prevMoves, Map map)
        {
            throw new NotImplementedException();
            var graph = new Graph(map);
            if (myMines.Any())
            {

                throw new NotImplementedException();
            }

            var queue = new Queue<QueueItem>();
            var used = new Dictionary<int, QueueItem>();
            foreach (var mineId in map.Mines.Where(id => !myMines.Contains(id)))
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
                foreach (var edge in current.CurrentVertex.Edges)
                {
                    var next = graph.Vertexes[edge.To];
                    QueueItem prev;
                    if (used.TryGetValue(next.Id, out prev))
                    {

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


            // bfs от всех, кроме myMines
            // if (has path)
            //     return first path item

            // foreach myMines.adjastentRivers take best

            // pass
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