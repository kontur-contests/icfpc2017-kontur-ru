using System;
using System.Collections.Generic;
using System.Linq;

namespace lib.GraphImpl.ShortestPath
{
    public class ShortestPathGraphWithOptions : ShortestPathGraph
    {
        public static ShortestPathGraphWithOptions Build(Graph graph, Func<Edge, bool> takeEdge, ICollection<int> sourceVertexes, int optionsCount)
        {
            var queue = new SortedSet<QueueItem>();
            var dist = CreateMap(new {OptionsUsed = 0, VertexId = 0}, 0);

            foreach (var sourceVertex in sourceVertexes)
            {
                queue.Add(new QueueItem(0,0,sourceVertex));
                dist[new {OptionsUsed = 0, VertexId = sourceVertex}] = 0;
            }

            while (queue.Any())
            {
                var item = queue.Min();
                queue.Remove(item);
                var from = item.VertexId;
                if(dist[new {item.OptionsUsed, item.VertexId}] != item.Distance)
                    continue;
                foreach (var edge in graph.Vertexes[from].Edges)
                {
                    if (!takeEdge(edge))
                        continue;
                    var to = edge.To;
                    var nextOptions = item.OptionsUsed + (edge.MustUseOption ? 1 : 0);
                    if(nextOptions > optionsCount)
                        continue;
                    var newDistance = item.Distance + 1;
                    if (dist.TryGetValue(new {OptionsUsed = nextOptions, VertexId = to}, out var curDistance) && curDistance <= newDistance)
                        continue;

                    dist[new {OptionsUsed = nextOptions, VertexId = to}] = newDistance;
                    queue.Add(new QueueItem(nextOptions, newDistance, to));
                }
            }

            var spGraph = new ShortestPathGraphWithOptions();
            var vertexDistances = dist.GroupBy(x => x.Key.VertexId)
                .ToDictionary(distances => distances.Key, distances => distances.Min(d => d.Value));
            foreach (var item in vertexDistances)
                spGraph.AddVertex(item.Key, item.Value);
            foreach (var from in spGraph.Vertexes.Values)
            {
                foreach (var edge in graph.Vertexes[from.Id].Edges)
                {
                    var to = spGraph[edge.To];
                    if(to.Distance == -1)
                        continue;
                    if(to.Distance == from.Distance && from.Id < to.Id)
                        spGraph.AddSameLayerEdge(edge);
                    if(to.Distance > from.Distance)
                        spGraph.AddEdge(edge);
                }
            }

            return spGraph;
        }

        private static Dictionary<TKey, TValue> CreateMap<TKey, TValue>(TKey sampleKey, TValue sampleValue)
        {
            return new Dictionary<TKey, TValue>();
        }

        private class QueueItem : IComparable<QueueItem>
        {
            public readonly int OptionsUsed;
            public readonly int Distance;
            public readonly int VertexId;

            public QueueItem(int optionsUsed, int distance, int vertexId)
            {
                OptionsUsed = optionsUsed;
                Distance = distance;
                VertexId = vertexId;
            }

            public int CompareTo(QueueItem other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;
                var optionsCountComparison = OptionsUsed.CompareTo(other.OptionsUsed);
                if (optionsCountComparison != 0) return optionsCountComparison;
                var distanceComparison = Distance.CompareTo(other.Distance);
                if (distanceComparison != 0) return distanceComparison;
                return VertexId.CompareTo(other.VertexId);
            }

            public override string ToString()
            {
                return $"OptionsUsed: {OptionsUsed}, Distance: {Distance}, VertexId: {VertexId}";
            }
        }
    }
}