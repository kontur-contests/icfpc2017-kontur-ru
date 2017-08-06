using System;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies
{
    public class FutureIsNowSetupStrategy : ISetupStrategy
    {
        private readonly double pathMultiplier;
        private readonly State state;
        private readonly Graph graph;
        private readonly MineDistCalculator mineDistCalculator;

        public FutureIsNowSetupStrategy(State state, IServices services, double pathMultiplier = 1)
        {
            this.pathMultiplier = pathMultiplier;
            this.state = state;
            graph = services.Get<Graph>();
            mineDistCalculator = services.Get<MineDistCalculator>();
        }

        public AiSetupDecision Setup()
        {
            var graphDiameterEstimation = (int)Math.Round(pathMultiplier * Math.Sqrt(state.map.Sites.Length));
            var length = graphDiameterEstimation;
            var path = new PathSelector(state.map, mineDistCalculator, length).SelectPath();
            var futures = new FuturesPositioner(state.map, graph, path, mineDistCalculator).GetFutures();
            return AiSetupDecision.Create(futures);
        }
    }
}