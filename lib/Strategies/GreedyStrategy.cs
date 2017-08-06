using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies
{
    public class GreedyStrategy : IStrategy
    {
        public GreedyStrategy(State state, IServices services)
        {
            PunterId = state.punter;
            MineDistCalulator = services.Get<MineDistCalculator>(state);
            GraphService = services.Get<GraphService>(state);
        }

        private MineDistCalculator MineDistCalulator { get; }
        private int PunterId { get; }
        private GraphService GraphService { get; }

        public List<TurnResult> NextTurns()
        {
            var graph = GraphService.Graph;

            var calculator = new ConnectedCalculator(graph, PunterId);
            var result = new List<TurnResult>();
            foreach (var vertex in graph.Vertexes.Values)
            foreach (var edge in vertex.Edges.Where(x => x.Owner == -1))
            {
                var fromMines = calculator.GetConnectedMines(edge.From);
                var toMines = calculator.GetConnectedMines(edge.To);
                var addScore = CalcVertexScore(toMines, fromMines, edge.From) + CalcVertexScore(fromMines, toMines, edge.To);
                result.Add(
                    new TurnResult
                    {
                        Estimation = addScore,
                        River = edge.River,
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