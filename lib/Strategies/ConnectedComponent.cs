using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;

namespace lib.Strategies
{
    public class ConnectedComponent
    {
        public HashSet<int> Vertices = new HashSet<int>();
        public HashSet<int> Mines = new HashSet<int>();
        public int Id { get; }
        public int OwnerPunterId { get; }

        public ConnectedComponent(int id, int ownerPunterId)
        {
            Id = id;
            OwnerPunterId = ownerPunterId;
        }

        public static List<ConnectedComponent> GetComponents(Graph graph, int owner)
        {
            var queue = new Queue<Vertex>();

            HashSet<int> usedMines = new HashSet<int>();

            var result = new List<ConnectedComponent>();

            int componentIndexer = 0;

            foreach (var mine in graph.Mines)
            {
                if (usedMines.Contains(mine.Key))
                    continue;

                var component = new ConnectedComponent(componentIndexer, owner);
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
