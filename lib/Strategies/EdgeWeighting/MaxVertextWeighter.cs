using System;
using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using lib.GraphImpl.ShortestPath;

namespace lib.Strategies.EdgeWeighting
{
    public class MaxVertextWeighter : IEdgeWeighter
    {
        public MaxVertextWeighter(MineDistCalculator mineDistCalculator, double mineWeight)
        {
            MineWeight = mineWeight;
            MineDistCalculator = mineDistCalculator;
        }

        private double MineWeight { get; }
        private Graph Graph { get; set; }
        private MineDistCalculator MineDistCalculator { get; }
        private ShortestPathGraph SpGraph { get; set; }
        private Dictionary<int, double> SubGraphWeight { get; set; }
        private int[] ClaimedMineIds { get; set; }

        public ConnectedComponent CurrentComponent => null;

        public void Init(Graph graph, List<ConnectedComponent> connectedComponents)
        {
            var claimedVertexIds = connectedComponents.SelectMany(comp => comp.Vertices).ToArray();
            Graph = graph;
            SpGraph = ShortestPathGraph.Build(graph, claimedVertexIds);
            ClaimedMineIds = claimedVertexIds.Where(v => graph.Mines.ContainsKey(v)).ToArray();
            SubGraphWeight = new Dictionary<int, double>();

            foreach (var claimedVertexId in claimedVertexIds)
                SubGraphWeight[claimedVertexId] = CalcSubGraphWeight(claimedVertexId);
        }

        public double EstimateWeight(Edge edge)
        {
            return SubGraphWeight.TryGetValue(edge.To, out var weight) ? weight : 0;
        }

        private double CalcSubGraphWeight(int vertexId)
        {
            if (SubGraphWeight.TryGetValue(vertexId, out var weight))
                return weight;
            weight = CalcVertexScore(vertexId);
            foreach (var edge in SpGraph.Vertexes[vertexId].Edges)
                weight = Math.Max(weight, CalcSubGraphWeight(edge.To));
            SubGraphWeight[vertexId] = weight;
            return weight;
        }

        private double CalcVertexScore(int vertexId)
        {
            return (Graph.Mines.ContainsKey(vertexId) ? MineWeight : 1) *
                   ClaimedMineIds.Select(mineId => MineDistCalculator.GetDist(mineId, vertexId))
                       .Sum(x => x * x);
        }
    }
}