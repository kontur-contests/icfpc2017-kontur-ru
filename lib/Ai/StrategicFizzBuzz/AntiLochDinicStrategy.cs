using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies;

namespace lib.Ai.StrategicFizzBuzz
{
    namespace lib.Strategies
    {
        public class AntiLochDinicStrategy : IStrategy
        {
            private static readonly ThreadLocal<Random> Random =
                new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

            private readonly Dictionary<Edge, double> edgesToBlock = new Dictionary<Edge, double>();

            public AntiLochDinicStrategy(State state, IServices services)
            {
                Graph = services.Get<Graph>();
                PunterId = state.punter;
            }

            private Graph Graph { get; }
            private int PunterId { get; }

            public List<TurnResult> NextTurns()
            {
                Init();

                return Graph.Vertexes.Values
                    .SelectMany(v => v.Edges)
                    .Select(
                        edge => new TurnResult
                        {
                            Estimation = EstimateWeight(edge),
                            Move = AiMoveDecision.Claim(PunterId, edge.River.Source, edge.River.Target)
                        })
                    .Where(river => river.Estimation > 0)
                    .ToList();
            }

            private void Init()
            {
                var mineToSave = Graph.Mines
                    .Where(mine => mine.Value.Edges.All(edge => edge.Owner != PunterId))
                    .FirstOrDefault(mine => mine.Value.Edges.Count(edge => edge.Owner < 0) < PunterId)
                    .Value;
                if (mineToSave != null)
                {
                    var edgeToSave = mineToSave.Edges.OrderBy(_ => Random.Value.Next())
                        .FirstOrDefault(edge => edge.Owner < 0);
                    if (edgeToSave != null)
                        edgesToBlock[edgeToSave] = 10;
                }

                var mines = Graph.Mines.Select(x => x.Key).ToList();
                foreach (var mine1 in mines)
                {
                    foreach (var mine2 in mines)
                    {
                        if (mine1 >= mine2)
                            continue;

                        var dinic = new Dinic(Graph, PunterId, mine1, mine2, out var flow);
                        if (flow != 1)
                            continue;

                        foreach (var edge in dinic.GetMinCut().Select(edge1 => edge1))
                            edgesToBlock[edge] = 1;
                    }
                }
            }

            private double EstimateWeight(Edge edge)
            {
                return edgesToBlock.GetOrDefault(edge, 0);
            }
        }
    }
}