using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies
{
    public class LochDinicKillerStrategy : IStrategy
    {
        private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
        private readonly Dictionary<Edge, double> edgesToBlock = new Dictionary<Edge, double>();

        public LochDinicKillerStrategy(State state, IServices services)
        {
            Graph = services.Get<Graph>();
            PunterId = state.punter;
            PuntersCount = state.punters;
        }

        private Graph Graph { get; }
        private int PunterId { get; }
        private int PuntersCount { get; }

        public List<TurnResult> NextTurns()
        {
            Init();

            return Graph.Vertexes.Values
                .SelectMany(v => v.Edges)
                .Select(
                    edge => edge.IsFree ? new TurnResult
                    {
                        Estimation = EstimateWeight(edge),
                        Move = AiMoveDecision.Claim(edge, PunterId),
                    } : null)
                .Where(river => river != null && river.Estimation > 0)
                .ToList();
        }

        private void Init()
        {
            var maxCount = 10;
            edgesToBlock.Clear();

            var mineToSave = Graph.Mines
                .Where(mine => mine.Value.Edges.All(edge => !edge.IsOwnedBy(PunterId)))
                .FirstOrDefault(mine => mine.Value.Edges.Count(edge => edge.IsFree) < PuntersCount)
                .Value;
            if (mineToSave != null)
            {
                var edgeToSave = mineToSave.Edges.OrderBy(_ => Random.Value.Next()).FirstOrDefault(edge => edge.IsFree);
                if (edgeToSave != null)
                    edgesToBlock[edgeToSave] = 10;
            }

            var bannedMines = Graph.Mines
                .Where(mine => mine.Value.Edges.SelectMany(edge => edge.GetOwners()).Distinct().Count() == PuntersCount)
                .Select(mine => mine.Key)
                .ToHashSet();

            var mines = Graph.Mines.Where(mine => mine.Value.Edges.Any(edge => edge.IsFree)).ToList();
            if (mines.Count >= 2)
            {
                for (var i = 0; i < Math.Min(10, mines.Count * (mines.Count - 1)); i++)
                {
                    var mine1 = mines[Random.Value.Next(mines.Count)];
                    var mine2 = mines[Random.Value.Next(mines.Count)];
                    while (mine2.Key == mine1.Key) mine2 = mines[Math.Min(Random.Value.Next(mines.Count), mines.Count - 1)];

                    // Чтобы не ходить по своим рёбрам в динице. Таким образом, длинные мосты блочаться один раз.
                    const int nonExistentPunterId = 1000000; 
                    var dinic = new Dinic(Graph, nonExistentPunterId, mine1.Key, mine2.Key, out var flow);
                    if (flow == 0)
                        continue;
                    if (flow > maxCount)
                        continue;

                    foreach (var edge in dinic.GetMinCut())
                    {
                        if (bannedMines.Contains(edge.From) || bannedMines.Contains(edge.To))
                            continue;
                        edgesToBlock[edge] = edgesToBlock.GetOrDefault(edge, 0) + 1.0 / flow;
                    }
                }
            }
        }

        private double EstimateWeight(Edge edge)
        {
            return edgesToBlock.GetOrDefault(edge, 0);
        }
    }
}