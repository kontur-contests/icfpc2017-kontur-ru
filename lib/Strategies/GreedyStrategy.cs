using System;
using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies
{
    public class GreedyStrategy : IStrategy
    {
        private readonly bool allowToUseOptions;

        public GreedyStrategy(bool allowToUseOptions, State state, IServices services, Func<long, long, long> aggregateEdgeScores)
        {
            this.allowToUseOptions = state.settings.options && allowToUseOptions && state.map.OptionsLeft(state.punter) > 0;
            AggregateEdgeScores = aggregateEdgeScores;
            PunterId = state.punter;
            MineDistCalulator = services.Get<MineDistCalculator>();
            Graph = services.Get<Graph>();
        }

        private Func<long, long, long> AggregateEdgeScores { get; }
        private MineDistCalculator MineDistCalulator { get; }
        private int PunterId { get; }
        private Graph Graph { get; }

        public List<TurnResult> NextTurns()
        {
            var calculator = new ConnectedCalculator(Graph, PunterId);
            var result = new List<TurnResult>();
            foreach (var vertex in Graph.Vertexes.Values)
            foreach (var edge in vertex.Edges.Where(x => x.CanBeOwnedBy(PunterId, allowToUseOptions)))
            {
                var fromMines = calculator.GetConnectedMines(edge.From);
                var toMines = calculator.GetConnectedMines(edge.To);
                var fromScore = CalcVertexScore(toMines, fromMines, edge.From);
                var toScore = CalcVertexScore(fromMines, toMines, edge.To);
                var addScore = AggregateEdgeScores(fromScore, toScore);
                result.Add(
                    new TurnResult
                    {
                        Estimation = addScore,
                        Move = AiMoveDecision.ClaimOrOption(edge, PunterId, allowToUseOptions)
                    });
            }
            return result;
        }

        private long CalcVertexScore(HashSet<int> mineIds, HashSet<int> usedMineIds, int vertexId)
        {
            return mineIds.Where(x => !usedMineIds.Contains(x))
                .Select(mineId => MineDistCalulator.GetDist(mineId, vertexId))
                .Sum(x => x * x);
        }
    }
}