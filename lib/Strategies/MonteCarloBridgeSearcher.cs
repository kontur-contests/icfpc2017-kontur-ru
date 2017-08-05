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
        public List<List<Edge>> Paths = new List<List<Edge>>();

        public MonteCarloBridgeSearcher(Graph graph, MineDistCalculator mineDistCalulator)
        {
            Graph = graph;
            this.mineDistCalulator = mineDistCalulator;
        }

        public void RemoveEdge(Edge edge)
        {
            Paths = Paths.Where(path => path.All(edge1 => edge1.From == edge.From && edge.To == edge1.To)).ToList();
        }

        public void BuildBridges()
        {
            foreach (var mine1 in Graph.Mines.Values)
            {
                foreach (var mine2 in Graph.Mines.Values.Where(mine2 => mine2.Id != mine1.Id))
                {
                    var edges = MonteCarloBridgeSearch(mine1, mine2);
                    foreach (var edge in edges)
                    {
                        Bridges[edge.Key] = Bridges.GetOrDefault(edge.Key, 0.0) + edge.Value;
                    }
                }
            }
        }

        Random rand = new Random();

        public Dictionary<Edge, double> MonteCarloBridgeSearch(Vertex mine1, Vertex mine2)
        {            
            Dictionary<Edge, int> usedEdgesCount = new Dictionary<Edge, int>();
            var pathsCount = 0;
            List<Edge> path = new List<Edge>();
            HashSet<int> usedNodes = new HashSet<int>();

            const int mcIterationscount = 100;

            for (int i = 0; i < mcIterationscount; i++)
            {
                path.Clear();
                usedNodes.Clear();
                usedNodes.Add(mine1.Id);
                var currentNode = mine1;
                var currentPathSize = mineDistCalulator.GetDist(mine2.Id, mine1.Id);

                for (int j = 0; j < 1000; j++)
                {
                    if (currentNode == mine2)
                    {
                        path.ForEach(edge => usedEdgesCount[edge] = usedEdgesCount.GetOrDefault(edge, 0) + 1);
                        pathsCount++;
                        break;
                    }
                    var possiblePaths = currentNode.Edges
                        .Where(edge => !usedNodes.Contains(edge.To) && mineDistCalulator.GetDist(mine2.Id, edge.To) <= currentPathSize + 1)
                        .ToList();

                    if(possiblePaths.Count == 0)
                        break;

                    var idx = rand.Next(Math.Min(possiblePaths.Count, possiblePaths.Count - 1));
                    
                    var choosenEdge = possiblePaths[idx];
                    path.Add(choosenEdge);
                    usedNodes.Add(choosenEdge.To);
                    currentNode = Graph.Vertexes[choosenEdge.To];
                    currentPathSize = Math.Min(mineDistCalulator.GetDist(mine2.Id, choosenEdge.To), currentPathSize);
                }   
            }
            return pathsCount == 0 ? new Dictionary<Edge, double>() : usedEdgesCount.ToDictionary(pair => pair.Key, pair => pair.Value*1.0/pathsCount);
        }

        public List<Edge> AStarSearch(int startIdx, int endIdx)
        {
            return null;
        }
    }
}
