using System;
using System.Linq;
using lib.Ai.StrategicFizzBuzz;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Ai
{
    public class LochMaxVertexWeighterKillerAi : IAi
    {
        private IAi Base = new MaxReachableVertexWithConnectedComponentsWeightAi();

        private Random rand = new Random();
        public string Name => nameof(LochMaxVertexWeighterKillerAi);
        public string Version => "0.2";

        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<GraphService>(state);
            return Base.Setup(state, services);
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            if (state.map.Sites.Length < 300)
                return Base.GetNextMove(state, services);
            //if (map.Sites.Length / puntersCount < 150)
            //  return Base.GetNextMove(prevMoves, map);

            var graph = services.Get<GraphService>(state).Graph;

            var nearMinesEdge = state.map.Mines
                .Select(mine => new {mine, edges = graph.Vertexes[mine].Edges.Select(edge => edge.River).ToList()})
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