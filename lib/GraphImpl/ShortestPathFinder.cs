using System.Collections.Generic;
using System.Linq;

namespace lib.GraphImpl
{
    public class ShortestPathFinder
    {
        private readonly Graph graph;
        readonly Dictionary<int, int> parrent = new Dictionary<int, int>();

        public ShortestPathFinder(Graph graph, int ownerId, List<int> starts)
        {
            this.graph = graph;

            Bfs(starts, ownerId);
        }

        public List<int> GetPath(int to)
        {
            if (!parrent.ContainsKey(to))
                return null;

            var result = new List<int>();
            while (to != -1)
            {
                result.Add(to);
                to = parrent[to];
            }
            return result;
        }

        private void Bfs(List<int> starts, int owner)
        {
            var dist = new Dictionary<int, int>();
            var queue = new Queue<int>();

            foreach (var start in starts)
            {
                dist[start] = 0;
                parrent[start] = -1;
                queue.Enqueue(start);
            }

            while (queue.Any())
            {
                int v = queue.Dequeue();
                foreach (var edge in graph.Vertexes[v].Edges)
                {
                    if (edge.IsOwnedBy(owner) || edge.IsFree)
                    {
                        int u = edge.To;
                        if (dist.ContainsKey(u))
                            continue;
                        dist.Add(u, dist[v] + 1);
                        parrent.Add(u, v);
                        queue.Enqueue(u);
                    }
                }
            }
        }
    }
}