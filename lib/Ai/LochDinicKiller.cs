using System;
using System.Collections.Generic;
using System.Linq;
using lib.Ai.StrategicFizzBuzz;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Ai
{
    public class LochDinicKiller : IAi
    {
        private IAi Base = new MaxReachableVertexWithConnectedComponentsWeightAi();

        private Random rand = new Random();
        public string Name => nameof(LochDinicKiller);
        public string Version => "0.2";

        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<GraphService>(state);
            return Base.Setup(state, services);
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            var graph = services.Get<GraphService>(state).Graph;

            int maxCount = 10;
            List<Edge> edgesToBlock = new List<Edge>();

            var mineToSave = graph.Mines
                .Where(mine => mine.Value.Edges.All(edge => edge.Owner != state.punter))
                .FirstOrDefault(mine => mine.Value.Edges.Count(edge => edge.Owner < 0) < state.punters).Value;
            if (mineToSave != null)
            {
                var edgeToSave = mineToSave.Edges.OrderBy(_ => rand.Next()).FirstOrDefault(edge => edge.Owner < 0);
                if (edgeToSave != null)
                    return AiMoveDecision.Claim(state.punter, edgeToSave.From, edgeToSave.To);
            }

            var bannedMines = graph.Mines
                .Where(mine => mine.Value.Edges.Select(edge => edge.Owner).Distinct().Count() == state.punters + 1)
                .Select(mine => mine.Key)
                .ToHashSet();

            var mines = graph.Mines.Where(mine => mine.Value.Edges.Any(edge => edge.Owner < 0)).ToList();
            for (int i = 0; i < Math.Min(10, mines.Count*(mines.Count - 1)); i++)
            {
                var mine1 = mines[Math.Min(rand.Next(mines.Count), mines.Count - 1)];
                var mine2 = mines[Math.Min(rand.Next(mines.Count), mines.Count - 1)];
                while(mine2.Key == mine1.Key) mine2 = mines[Math.Min(rand.Next(mines.Count), mines.Count - 1)];

                var dinic = new Dinic(graph, state.punter, mine1.Key, mine2.Key, out var flow);
                if (flow == 0)
                    continue;
                if (flow > maxCount)
                    continue;
                edgesToBlock.AddRange(dinic.GetMinCut().Where(edge => !bannedMines.Contains(edge.From)));
            }

            edgesToBlock = edgesToBlock.Distinct().ToList();

            if (edgesToBlock.Count == 0)
                return Base.GetNextMove(state, services);
            var choosenEdge = edgesToBlock[Math.Min(edgesToBlock.Count - 1, rand.Next(edgesToBlock.Count))]; 
            return AiMoveDecision.Claim(state.punter, choosenEdge.From, choosenEdge.To);
        }
    }
}