using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies
{
    public class CrazyStrategy : IStrategy
    {
        private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        public CrazyStrategy(State state, IServices services)
        {
            Graph = services.Get<Graph>();
            MineDistCalculator = services.Get<MineDistCalculator>();
            PunterId = state.punter;
        }

        private Graph Graph { get; }
        private MineDistCalculator MineDistCalculator { get; }
        private int PunterId { get; }

        public List<TurnResult> NextTurns()
        {
            var result = new List<TurnResult>();
            var mines = Graph.Mines.Keys.ToList();
            for (var i = 0; i < mines.Count; i++)
            for (var j = i + 1; j < mines.Count; j++)
            {
                var denic = new Dinic(Graph, PunterId, mines[i], mines[j], out int flow);
                if (flow == 0 || flow == Dinic.INF)
                    continue;
                var cut = denic.GetMinCut();
                foreach (var edge in cut)
                    result.Add(
                        new TurnResult
                        {
                            Estimation = Random.Value.NextDouble(),
                            Move = AiMoveDecision.Claim(edge, PunterId, "Because I'm crazy!")
                        });
            }
            return result;
        }
    }
}