﻿using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;
using MoreLinq;

namespace lib.Strategies.EdgeWeighting
{
    public class BiggestComponentEWStrategy : IStrategy
    {
        public BiggestComponentEWStrategy(IEdgeWeighter edgeWeighter, State state, IServices services)
        {
            PunterId = state.punter;
            EdgeWeighter = edgeWeighter;
            MineDistCalulator = services.Get<MineDistCalculator>();
            ConnectedComponentsService = services.Get<ConnectedComponentsService>();
            Graph = services.Get<Graph>();
        }

        private MineDistCalculator MineDistCalulator { get; }
        private IEdgeWeighter EdgeWeighter { get; }
        private int PunterId { get; }
        private ConnectedComponentsService ConnectedComponentsService { get; }
        private Graph Graph { get; }

        public AiSetupDecision Setup()
        {
            return null;
        }

        public List<TurnResult> NextTurns()
        {
            var claimedVertexes = Graph.Vertexes.Values
                .SelectMany(x => x.Edges)
                .Where(edge => edge.Owner == PunterId)
                .SelectMany(edge => new[] {edge.From, edge.To})
                .Distinct()
                .ToArray();

            if (claimedVertexes.Length == 0)
                return Graph.Mines.Values
                    .SelectMany(v => v.Edges)
                    .Where(e => e.Owner == -1)
                    .Select(
                        e => new TurnResult
                        {
                            Estimation = 1,
                            Move = AiMoveDecision.Claim(PunterId, e.River.Source, e.River.Target, "No edges yet, selecting first mine")
                        })
                    .ToList();

            var connectedComponents = ConnectedComponentsService.For(PunterId);
            var maxComponent = connectedComponents.MaxBy(comp => comp.Vertices.Count);
            EdgeWeighter.Init(connectedComponents, maxComponent);
            return maxComponent.Vertices
                .SelectMany(v => Graph.Vertexes[v].Edges)
                .Where(e => e.Owner == -1 && !AreConnected(maxComponent, e.From, e.To))
                .Select(
                    e => new TurnResult
                    {
                        Estimation = EdgeWeighter.EstimateWeight(e),
                        Move = AiMoveDecision.Claim(PunterId, e.River.Source, e.River.Target)
                    })
                .Where(t => t.Estimation > 0)
                .ToList();
        }

        private bool AreConnected(ConnectedComponent currentComponent, int fromId, int toId)
        {
            return currentComponent.Vertices.Contains(fromId) && currentComponent.Vertices.Contains(toId);
        }
    }
}