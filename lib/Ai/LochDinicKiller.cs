using System;
using System.Collections.Generic;
using System.Linq;
using lib.Ai.StrategicFizzBuzz;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies.EdgeWeighting;
using MoreLinq;

namespace lib.Ai
{
    public class LochDinicKiller : IAi
    {
        private IAi Base = new AgileMaxVertexWeighterAi();

        private Random rand = new Random();
        public string Name => nameof(LochDinicKiller);
        public string Version => "0.3";

        DinicWeighter dinicWeighter = new DinicWeighter();

        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<Graph>();
            return Base.Setup(state, services);
        }

        private Tuple<int, int> ConvertToTuple(Edge edge)
        {
            return edge.From > edge.To ? Tuple.Create(edge.To, edge.From) : Tuple.Create(edge.From, edge.To);
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            dinicWeighter.Init(state, services, null, null);

            var graph = services.Get<GraphService>(state).Graph;

            int maxCount = 10;
            Dictionary<Tuple<int, int>, double> edgesToBlock = new Dictionary<Tuple<int, int>, double>();

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
                
                foreach (var edge in dinic.GetMinCut().Select(ConvertToTuple))
                {
                    if(bannedMines.Contains(edge.Item1) || bannedMines.Contains(edge.Item2))
                        continue;
                    edgesToBlock[edge] = edgesToBlock.GetOrDefault(edge, 0) + 1.0 / flow;
                }
            }

            if (edgesToBlock.Count == 0)
                return Base.GetNextMove(state, services);
            var choosenEdge = edgesToBlock.MaxBy(edge => edge.Value).Key; 
            return AiMoveDecision.Claim(state.punter, choosenEdge.Item1, choosenEdge.Item2);
        }
    }
}