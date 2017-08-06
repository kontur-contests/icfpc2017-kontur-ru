using System;
using System.Collections.Generic;
using System.Linq;
using lib.Ai.StrategicFizzBuzz;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies.EdgeWeighting;
using lib.Structures;
using MoreLinq;

namespace lib.Ai
{
    public class KillerFromFuture : IAi
    {
        private IAi Base = new AgileMaxVertexWeighterAi();

        private Random rand = new Random();
        public string Name => GetType().Name;
        public string Version => "0.3";

        private double pathMultiplier;

        DinicWeighter dinicWeighter;

        public KillerFromFuture()
            :this(1)
        {
        }

        public KillerFromFuture(double pathMultiplier)
        {
            this.pathMultiplier = pathMultiplier;
        }

        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<Graph>();
            new DinicWeighter(state, services);
            var graph = services.Get<Graph>();
            var mineDists = services.Get<MineDistCalculator>();

            if (!state.settings.futures)
                return AiSetupDecision.Create(new Future[0]);

            var graphDiameterEstimation = (int)Math.Round(pathMultiplier * Math.Sqrt(state.map.Sites.Length));
            var length = graphDiameterEstimation;
            var path = new PathSelector(state.map, mineDists, length).SelectPath();
            var futures = new FuturesPositioner(state.map, graph, path, mineDists).GetFutures();
            return AiSetupDecision.Create(futures);
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
                .Select(river => new { river, weight = dinicWeighter.EstimateWeight(new Edge(river.Source, river.Target, river.Owner)) })
                .Where(river => river.weight > 0)
                .ToList();

            if (edgeToBlock.Count == 0)
                return Base.GetNextMove(state, services);
            var choosenEdge = edgeToBlock.MaxBy(edge => edge.weight).river;
            return AiMoveDecision.Claim(state.punter, choosenEdge.Source, choosenEdge.Target);
        }
    }
}