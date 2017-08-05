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
        public int Id { get; }

        public ConnectedComponent(int id)
        {
            Id = id;
        }

        public static List<ConnectedComponent> GetComponents(Graph graph, int owner)
        {
            var queue = new Queue<Vertex>();

            HashSet<int> usedMines = new HashSet<int>();

            var result = new List<ConnectedComponent>();

            int componentIndexer = 0;

            foreach (var mine in graph.Mines)
            {
                if(mine.Value.Edges.All(edge => edge.Owner == owner) || usedMines.Contains(mine.Key))
                    continue;
                
                var component = new ConnectedComponent(componentIndexer);
                componentIndexer++;

                queue.Clear();

                component.Vertices.Add(mine.Key);
                component.Mines.Add(mine.Key);
                usedMines.Add(mine.Key);
                queue.Enqueue(mine.Value);

                while (queue.Count > 0)
                {
                    var node = queue.Dequeue();
                    foreach (var edge in node.Edges.Where(edge => edge.Owner == owner).Where(edge => !component.Vertices.Contains(edge.To)))
                    {
                        var edgeNode = graph.Vertexes[edge.To];
                        if (edgeNode.IsMine)
                        {
                            component.Mines.Add(edgeNode.Id);
                            usedMines.Add(edgeNode.Id);
                        }
                        component.Vertices.Add(edgeNode.Id);
                        queue.Enqueue(edgeNode);
                    }
                }
                result.Add(component);
            }
            return result;
        }
    }
}
