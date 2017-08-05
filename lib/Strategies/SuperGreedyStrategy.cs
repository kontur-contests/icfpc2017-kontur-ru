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
        private MineDistCalculator MineDistCalulator;

        public SuperGreedyStrategy(ShortestPathGraph shortestPathGraph, MineDistCalculator mineDistCalulator)
        {
            MineDistCalulator = mineDistCalulator;
        }

        public List<TurnResult> Turn(Graph graph)
        {
            throw new NotImplementedException();
        }
    }

    class ConnectedComponent
    {
        public HashSet<int> Vertices = new HashSet<int>();
        public HashSet<int> Mines = new HashSet<int>();

        public ConnectedComponent(Graph graph, int owner)
        {
            var queue = new Queue<Vertex>();

            foreach (var mine in graph.Mines)
            {
                if(mine.Value.Edges.All(edge => edge.Owner == owner))
                    break;

                queue.Clear();

                Vertices.Add(mine.Key);
                Mines.Add(mine.Key);
                queue.Enqueue(mine.Value);

                while (queue.Count > 0)
                {
                    var node = queue.Dequeue();
                    foreach (var edge in node.Edges.Where(edge => edge.Owner == owner).Where(edge => !Vertices.Contains(edge.To)))
                    {
                        var edgeNode = graph.Vertexes[edge.To];
                        if (edgeNode.IsMine)
                            Mines.Add(edgeNode.Id);
                        Vertices.Add(edgeNode.Id);
                        queue.Enqueue(edgeNode);
                    }
                }
            }
        }
    }
}
