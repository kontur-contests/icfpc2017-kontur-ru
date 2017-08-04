using System;
using System.Collections.Generic;
using System.Linq;

namespace lib.GraphImpl
{
    public class ConnectedCalculator
    {
        private readonly Graph graph;
        private readonly Dictionary<int, List<int>> minesConnected;

        public ConnectedCalculator(Graph graph, int ownerId)
        {
            this.graph = graph;

            minesConnected = new Dictionary<int, List<int>>();
            foreach (var vertex in graph.Mines.Values)
            {
                Bfs(vertex.Id, ownerId);
            }
        }

        public List<int> GetConnectedMines(int v)
        {
            return minesConnected.ContainsKey(v)
                ? minesConnected[v]
                : new List<int>();
        }

        private void Bfs(int start, int owner)
        {
            var dist = new Dictionary<int, int>();
            var queue = new Queue<int>();

            dist[start] = 0;
            queue.Enqueue(start);

            while (queue.Any())
            {
                int v = queue.Dequeue();
                foreach (var edge in graph.Vertexes[v].Edges)
                {
                    if (edge.Owner != owner)
                        continue;

                    int u = edge.To;
                    if (dist.ContainsKey(u))
                        continue;
                    dist.Add(u, dist[v] + 1);
                    queue.Enqueue(u);
                }
            }

            foreach (var v in dist.Keys)
            {
                if (!minesConnected.ContainsKey(v))
                    minesConnected.Add(v, new List<int>());
                minesConnected[v].Add(start);
            }
        }
    }
}