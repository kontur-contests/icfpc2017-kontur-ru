using System;
using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Structures;

namespace lib.Strategies
{
    // Dont use!
    public class FutureConnectivityDependentSetupStrategy : ISetupStrategy
    {
        private readonly Graph graph;
        private readonly MineDistCalculator mineDistCalculator;
        private readonly double pathMultiplier;
        private readonly State state;
        private FutureIsNowSetupStrategy futureIsNowSetupStrategy;

        public FutureConnectivityDependentSetupStrategy(
            double pathMultiplier, State state, Graph graph, MineDistCalculator mineDistCalculator)
        {
            futureIsNowSetupStrategy = new FutureIsNowSetupStrategy(pathMultiplier, state, graph, mineDistCalculator);
            this.state = state;
            this.graph = graph;
            this.mineDistCalculator = mineDistCalculator;
        }

        public AiSetupDecision Setup()
        {
            if (GetConnectivity() > Math.PI)
                return futureIsNowSetupStrategy.Setup();
            return AiSetupDecision.Create(new Future[0], "connectivity is too low :-(");
        }

        private double GetConnectivity()
        {
            return new ConnectivityCalculator(graph).CutSizeForEachMinePair().Average(c => c.CutSize);
        }
    }
}