using System;
using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using lib.GraphImpl.ShortestPath;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class MaxVertextWeighter : IEdgeWeighter
    {
        public MaxVertextWeighter(double mineMultiplier, State state, IServices services)
        {
            MineDistCalculator = services.Get<MineDistCalculator>();
            Graph = services.Get<Graph>();
            SpGraphService = services.Get<ShortestPathGraphService>();
            MineMultiplier = mineMultiplier;
            State = state;
        }

        private State State;

        private double MineMultiplier { get; }
        private Graph Graph { get; }
        private ShortestPathGraphService SpGraphService { get; }
        private MineDistCalculator MineDistCalculator { get; }
        
        private ShortestPathGraph SpGraph { get; set; }
        private Dictionary<int, double> SubGraphWeight { get; set; }
        private ICollection<int> ClaimedMineIds { get; set; }
        private Dictionary<int, ConnectedComponent> VertexComponent { get; set; }
        private ConnectedComponent CurrentComponent { get; set; }
        private Dictionary<Tuple<int, int>, long> MutualComponentWeights { get; set; }

        public void Init(ConnectedComponent[] connectedComponents, ConnectedComponent currentComponent)
        {
            SubGraphWeight = new Dictionary<int, double>();
            CurrentComponent = currentComponent;
            VertexComponent = connectedComponents
                .SelectMany(x => x.Vertices, (component, vertex) => new {component, vertex})
                .ToDictionary(x => x.vertex, x => x.component);
            MutualComponentWeights = new Dictionary<Tuple<int, int>, long>();
            SpGraph = SpGraphService.ForComponent(CurrentComponent, VertexComponent);
            ClaimedMineIds = CurrentComponent.Mines;
            foreach (var edge in CurrentComponent.Vertices.SelectMany(v => Graph.Vertexes[v].Edges))
                SubGraphWeight[edge.To] = CalcSubGraphWeight(edge.To);
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
            foreach (var edge in SpGraph[vertexId].Edges)
//                weight = weight + CalcSubGraphWeight(edge.To);
                weight = Math.Max(weight, CalcSubGraphWeight(edge.To));
            SubGraphWeight[vertexId] = weight;
            return weight;
        }

        private long CalcVertexScore(int vertexId)
        {
            if (VertexComponent.TryGetValue(vertexId, out var component))
            {
                if (component.Id == CurrentComponent.Id)
                    return 0;
                if (!MutualComponentWeights.TryGetValue(Tuple.Create(component.Id, CurrentComponent.Id), out var weight))
                    weight = CalcMutualComponentWeight(component, CurrentComponent);
                return weight;
            }
            var vertexWeight = CalcProperVertexScore(vertexId, ClaimedMineIds);
            if (Graph.Mines.ContainsKey(vertexId))
                return (long) (MineMultiplier * vertexWeight) + CurrentComponent.Vertices.Sum(v => CalcProperVertexScore(v, new[] {vertexId}));
            return vertexWeight;
        }

        private int CalcProperVertexScore(int vertexId, ICollection<int> claimedMineIds)
        {
            return claimedMineIds.Select(
                mineId => MineDistCalculator.GetDist(mineId, vertexId))
                    .Sum(x => GetScore(claimedMineIds, x, vertexId));
        }

        private int GetScore(ICollection<int> claimedMineIds, int length, int vertexId)
        {
            if (State.settings.futures && State.aiSetupDecision.futures
                    .Any(future => claimedMineIds.Contains(future.source) && future.target == vertexId))
                return length * length * length + length * length;
            return length * length;
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