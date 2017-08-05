using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.GraphImpl;
using lib.GraphImpl.ShortestPath;

namespace lib.Strategies
{
    class SuperGreedyStrategy : IStrategy
    {
        public Graph Graph;
        public ShortestPathGraph ShortestPathGraph;
        private MineDistCalculator MineDistCalulator;

        public SuperGreedyStrategy(Graph graph, ShortestPathGraph shortestPathGraph, MineDistCalculator mineDistCalulator)
        {
            Graph = graph;
            ShortestPathGraph = shortestPathGraph;
            MineDistCalulator = mineDistCalulator;
        }

        public List<TurnResult> Turn(Graph graph)
        {
            throw new NotImplementedException();
        }
    }
}
