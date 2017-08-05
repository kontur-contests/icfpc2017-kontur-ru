using System;
using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using lib.GraphImpl.ShortestPath;
using MoreLinq;

namespace lib.Strategies.EdgeWeighting
{
    public class MaxVertextWeighterWithConnectedComponents : IEdgeWeighter
    {
        public MaxVertextWeighterWithConnectedComponents(Map map)
        {
            MineDistCalculator = new MineDistCalculator(new Graph(map));
        }

        private Graph Graph { get; set; }
        private MineDistCalculator MineDistCalculator { get; }
        private ShortestPathGraph SpGraph { get; set; }
        private Dictionary<int, double> SubGraphWeight { get; set; }
        private ICollection<int> ClaimedMineIds { get; set; }
        private Dictionary<int, ConnectedComponent> VertexComponent { get; set; }
        private ConnectedComponent CurrentComponent { get; set; }
        private Dictionary<Tuple<int, int>, long> MutualComponentWeights { get; set; }

        public void Init(Graph graph, int[] claimedVertexIds, int ownerId)
        {
            Graph = graph;
            SubGraphWeight = new Dictionary<int, double>();

            var connectedComponents = ConnectedComponent.GetComponents(Graph, ownerId);
            VertexComponent = connectedComponents
                .SelectMany(x => x.Vertices, (component, vertex) => new { component, vertex })
                .ToDictionary(x => x.vertex, x => x.component);
            MutualComponentWeights = new Dictionary<Tuple<int, int>, long>();
            var maxComponent = connectedComponents.MaxBy(comp => comp.Vertices.Count);
            {
                CurrentComponent = maxComponent;
                SpGraph = ShortestPathGraph.Build(Graph, maxComponent.Vertices);
                ClaimedMineIds = maxComponent.Mines;
                foreach (var vertex in maxComponent.Vertices)
                    SubGraphWeight[vertex] = CalcSubGraphWeight(vertex);
            }
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
                weight = Math.Max((double) weight, CalcSubGraphWeight(edge.To));
            SubGraphWeight[vertexId] = weight;
            return weight;
        }

        private long CalcVertexScore(int vertexId)
        {
            if (VertexComponent.TryGetValue(vertexId, out var component))
            {
                if (component.Id == CurrentComponent.Id)
                    return 0;
                if (!MutualComponentWeights.TryGetValue(Tuple.Create<int, int>(component.Id, CurrentComponent.Id), out var weight))
                    weight = CalcMutualComponentWeight(component, CurrentComponent);
                return weight;
            }
            var multiplier = 1;
            if (Graph.Mines.ContainsKey(vertexId))
                multiplier = 100;

            return multiplier * CalcProperVertexScore(vertexId, ClaimedMineIds);
        }

        private int CalcProperVertexScore(int vertexId, ICollection<int> claimedMineIds)
        {
            return claimedMineIds.Select(mineId => MineDistCalculator.GetDist(mineId, vertexId)).Sum(x => x * x);
        }

        private long CalcMutualComponentWeight(ConnectedComponent x, ConnectedComponent y)
        {
            var weight = 0;
            weight += x.Vertices.Sum(v => CalcProperVertexScore(v, y.Mines));
            weight += y.Vertices.Sum(v => CalcProperVertexScore(v, x.Mines));
            MutualComponentWeights.Add(Tuple.Create(x.Id, y.Id), weight);
            MutualComponentWeights.Add(Tuple.Create(y.Id, x.Id), weight);
            return weight;
        }
    }
}