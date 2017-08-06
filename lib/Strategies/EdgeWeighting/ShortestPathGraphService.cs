using System;
using System.Collections.Generic;
using lib.GraphImpl;
using lib.GraphImpl.ShortestPath;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class ShortestPathGraphService : IService
    {
        private readonly Graph graph;
        private readonly ConnectedComponentsService connectedComponentsService;
        private readonly IDictionary<Tuple<int, int>, ShortestPathGraph> cache = new Dictionary<Tuple<int, int>, ShortestPathGraph>();

        public ShortestPathGraphService(Graph graph, ConnectedComponentsService connectedComponentsService)
        {
            this.graph = graph;
            this.connectedComponentsService = connectedComponentsService;
        }

        public ShortestPathGraph For(int punterId, int componentId)
        {
            return cache.GetOrCreate(
                Tuple.Create(punterId, componentId), key =>
                {
                    var components = connectedComponentsService.For(punterId);
                    return ShortestPathGraph.Build(graph, components[componentId].Vertices);
                });
        }
    }
}