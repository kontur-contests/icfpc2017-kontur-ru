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

        public FutureIsNowSetupStrategy(double pathMultiplier, State state, Graph graph, MineDistCalculator mineDistCalculator)
        {
            this.pathMultiplier = pathMultiplier;
            this.state = state;
            this.graph = graph;
            this.mineDistCalculator = mineDistCalculator;
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