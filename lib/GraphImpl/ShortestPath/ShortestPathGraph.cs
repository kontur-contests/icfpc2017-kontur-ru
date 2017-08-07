using System;
using System.Collections.Generic;
using System.Linq;

namespace lib.GraphImpl.ShortestPath
{
    public class ShortestPathGraph
    {
        protected ShortestPathGraph()
        {
        }

        private readonly Dictionary<int, ShortestPathVertex> vertexes = new Dictionary<int, ShortestPathVertex>();

        public ShortestPathVertex this[int vertexId] => vertexes.TryGetValue(vertexId, out var vertex) ? vertex : new ShortestPathVertex(vertexId, -1);
        public ICollection<ShortestPathVertex> Vertexes => vertexes.Values;

        public static ShortestPathGraph Build(Graph graph, Func<Edge, bool> takeEdge, ICollection<int> sourceVertexes)
        {
            var spGraph = new ShortestPathGraph();
            var queue = new Queue<ShortestPathVertex>();
            foreach (var sourceVertex in sourceVertexes)
                queue.Enqueue(spGraph.AddVertex(sourceVertex, 0));

            while (queue.Any())
            {
                var from = queue.Dequeue();
                foreach (var edge in graph.Vertexes[from.Id].Edges)
                {
                    if (!takeEdge(edge))
                        continue;
                    if (spGraph.vertexes.ContainsKey(edge.To))
                    {
                        var to = spGraph.vertexes[edge.To];
                        if (to.Distance == from.Distance + 1)
                            spGraph.AddEdge(edge);
                        else if (to.Distance == from.Distance && from.Id < to.Id)
                            spGraph.AddSameLayerEdge(edge);
                        continue;
                    }
                    spGraph.AddEdge(edge);
                    queue.Enqueue(spGraph.AddVertex(edge.To, from.Distance + 1));
                }
            }

            return spGraph;
        }

        protected ShortestPathVertex AddVertex(int vertexId, int distance)
        {
            return vertexes[vertexId] = new ShortestPathVertex(vertexId, distance);
        }

        protected void AddEdge(Edge edge)
        {
            vertexes[edge.From].Edges.Add(edge);
        }

        protected void AddSameLayerEdge(Edge edge)
        {
            vertexes[edge.From].SameLayerEdges.Add(edge);
            vertexes[edge.To].SameLayerEdges.Add(edge.Reverse());
        }
    }
}