using System;
using System.Collections.Generic;
using lib.GraphImpl;
using lib.GraphImpl.ShortestPath;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class ShortestPathGraphService : IService
    {
        private Graph Graph { get; set; }
        private ConnectedComponentsService ConnectedComponentsService { get; set; }
        private IDictionary<Tuple<int, int>, ShortestPathGraph> ComponentsCache { get; } = new Dictionary<Tuple<int, int>, ShortestPathGraph>();
        private IDictionary<int, ShortestPathGraph> VertexesCache { get; } = new Dictionary<int, ShortestPathGraph>();

        public ShortestPathGraphService(Graph graph, ConnectedComponentsService connectedComponentsService)
        {
            this.graph = graph;
            this.connectedComponentsService = connectedComponentsService;
        }

        public ShortestPathGraph ForComponent(int punterId, int componentId)
        {
            return ComponentsCache.GetOrCreate(
                Tuple.Create(punterId, componentId), key =>
                {
                    var components = connectedComponentsService.For(punterId);
                    return ShortestPathGraph.Build(graph, components[componentId].Vertices);
                });
        }

        public ShortestPathGraph ForVertex(int vertexId)
        {
            return VertexesCache.GetOrCreate(vertexId, key => ShortestPathGraph.Build(Graph, vertexId));
    }
    }
}