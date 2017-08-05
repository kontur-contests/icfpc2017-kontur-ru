using System;
using System.Linq;
using lib.Ai.StrategicFizzBuzz;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Ai
{
    public class LochKillerAi : IAi
    {
        public string Name => nameof(LochKillerAi);
        public string Version => "0.1";

        private GreedyAi Base = new GreedyAi();

        Random rand = new Random();

        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<GraphService>(state);
            return Base.Setup(state, services);
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            if (state.map.Sites.Length < 300)
                return Base.GetNextMove(state, services);

            var graph = services.Get<GraphService>(state).Graph;
            var playersCount = state.map.Rivers.Select(river => river.Owner).Distinct().Count(i => i >= 0);

                var nearMinesEdge = state.map.Mines
                    .Select(mine => new { mine, edges = graph.Vertexes[mine].Edges.Select(edge => edge.River).ToList() })
                .Where(mine => mine.edges.Select(edge => edge.Owner).Distinct().Count() < playersCount+1)
                    .OrderBy(mine => Tuple.Create(mine.edges.Select(edge => edge.Owner).Distinct().Count(), rand.Next()))
                    .Where(mine => mine.edges.Count <= 100)
                    .SelectMany(mine => mine.edges)
                    .FirstOrDefault(edge => edge.Owner < 0);
                if (nearMinesEdge == null)
                    return Base.GetNextMove(state, services);
                return AiMoveDecision.Claim(state.punter, nearMinesEdge.Source, nearMinesEdge.Target);
        }
    }

    public class LochMaxVertexWeighterKillerAi : IAi
    {
        public string Name => nameof(LochMaxVertexWeighterKillerAi);
        public string Version => "0.1";

        private IAi Base = new MaxReachableVertexWithConnectedComponentsWeightAi();

        Random rand = new Random();

        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<GraphService>(state);
            return Base.Setup(state, services);
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            if (state.map.Sites.Length / state.punters < 150)
                return Base.GetNextMove(state, services);

            var graph = services.Get<GraphService>(state).Graph;

            var nearMinesEdge = state.map.Mines
                .Select(mine => new { mine, edges = graph.Vertexes[mine].Edges.Select(edge => edge.River).ToList() })
                .Where(mine => mine.edges.Select(edge => edge.Owner).Distinct().Count() < state.punters + 1)
                .OrderBy(mine => Tuple.Create(mine.edges.Select(edge => edge.Owner).Distinct().Count(), rand.Next()))
                .Where(mine => mine.edges.Count <= 100)
                .SelectMany(mine => mine.edges)
                .FirstOrDefault(edge => edge.Owner < 0);
            if (nearMinesEdge == null)
                return Base.GetNextMove(state, services);
            return AiMoveDecision.Claim(state.punter, nearMinesEdge.Source, nearMinesEdge.Target);
        }
    }
}