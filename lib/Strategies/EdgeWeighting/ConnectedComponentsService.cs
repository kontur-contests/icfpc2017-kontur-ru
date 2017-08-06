using System.Collections.Generic;
using lib.GraphImpl;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class ConnectedComponentsService : IService
    {
        private readonly Graph graph;
        private readonly IDictionary<int, List<ConnectedComponent>> cache = new Dictionary<int, List<ConnectedComponent>>();

        public ConnectedComponentsService(Graph graph)
        {
            this.graph = graph;
        }
        
        public List<ConnectedComponent> For(int punterId)
        {
            return cache.GetOrCreate(punterId, key => ConnectedComponent.GetComponents(graph, punterId));
        }
    }
}