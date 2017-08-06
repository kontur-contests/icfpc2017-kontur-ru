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
        public string Name => GetType().Name;
        public string Version => "0.3";

        DinicWeighter dinicWeighter;

        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<Graph>();
            new DinicWeighter(state, services);
            return Base.Setup(state, services);
        }

        private Tuple<int, int> ConvertToTuple(Edge edge)
        {
            return edge.From > edge.To ? Tuple.Create(edge.To, edge.From) : Tuple.Create(edge.From, edge.To);
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            dinicWeighter = new DinicWeighter(state, services);
            dinicWeighter.Init(null, null);
            
            var edgeToBlock = state.map.Rivers
                .Select(river => new{river, weight = dinicWeighter.EstimateWeight(new Edge(river.Source, river.Target, river.Owner)) })
                .Where(river => river.weight > 0)
                .ToList();

            if (edgeToBlock.Count == 0)
                return Base.GetNextMove(state, services);
            var choosenEdge = edgeToBlock.MaxBy(edge => edge.weight).river; 
            return AiMoveDecision.Claim(state.punter, choosenEdge.Source, choosenEdge.Target);
        }
    }
}