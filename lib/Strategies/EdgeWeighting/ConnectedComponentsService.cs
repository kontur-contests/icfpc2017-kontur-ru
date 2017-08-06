using System.Collections.Generic;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class ConnectedComponentsService : IService
    {
        private readonly Graph graph;
        private readonly IDictionary<int, ConnectedComponent[]> cache = new Dictionary<int, ConnectedComponent[]>();

        public ConnectedComponentsService(Graph graph)
        {
            this.graph = graph;
        }
        
        public ConnectedComponent[] For(int punterId)
        {
            return cache.GetOrCreate(punterId, key => ConnectedComponent.GetComponentsFromMines(graph, punterId).ToArray());
        }
    }
}