using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.GraphImpl;

namespace lib.Strategies
{
    class MonteCarloBridgeSearcher
    {
        public Graph Graph;
        private MineDistCalculator mineDistCalulator;

        public Dictionary<Edge, double> Bridges = new Dictionary<Edge, double>();

        public MonteCarloBridgeSearcher(Graph graph, MineDistCalculator mineDistCalulator)
        {
            Graph = graph;
            this.mineDistCalulator = mineDistCalulator;
        }
        
        public void BuildBridges(double measure)
        {
            foreach (var mine1 in Graph.Mines.Values)
            {
                foreach (var mine2 in Graph.Mines.Values.Where(mine2 => mine2 != mine1))
                {

                }
            }
        }

        public Dictionary<Edge, double> MonteCarloBridgeSearch(Vertex mine1, Vertex mine2, double measure)
        {
            throw new NotImplementedException();
        }
    }
}
