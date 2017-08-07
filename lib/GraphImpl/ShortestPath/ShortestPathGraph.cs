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

        public readonly Dictionary<int, ShortestPathVertex> Vertexes = new Dictionary<int, ShortestPathVertex>();
        public ShortestPathVertex this[int vertexId] => Vertexes.TryGetValue(vertexId, out var vertex) ? vertex : new ShortestPathVertex(vertexId, -1);

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
                    if (spGraph.Vertexes.ContainsKey(edge.To))
                    {
                        var to = spGraph.Vertexes[edge.To];
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
            return Vertexes[vertexId] = new ShortestPathVertex(vertexId, distance);
        }

        protected void AddEdge(Edge edge)
        {
            Vertexes[edge.From].Edges.Add(edge);
        }

        protected void AddSameLayerEdge(Edge edge)
        {
            Vertexes[edge.From].SameLayerEdges.Add(edge);
            Vertexes[edge.To].SameLayerEdges.Add(edge.Reverse());
        }
    }
}