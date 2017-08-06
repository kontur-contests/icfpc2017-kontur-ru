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

        public void Setup(State state, IServices services)
        {
            services.Setup<GraphService>(state);
            services.Setup<ConnectedComponentsService>(state);
        }

        public void ApplyNextState(State state, IServices services)
        {
            Graph = services.Get<GraphService>(state).Graph;
            ConnectedComponentsService = services.Get<ConnectedComponentsService>(state);
        }

        public ShortestPathGraph ForComponent(int punterId, int componentId)
        {
            return ComponentsCache.GetOrCreate(
                Tuple.Create(punterId, componentId), key =>
                {
                    var components = ConnectedComponentsService.For(punterId);
                    return ShortestPathGraph.Build(Graph, components[componentId].Vertices);
                });
        }

        public ShortestPathGraph ForVertex(int vertexId)
        {
            return VertexesCache.GetOrCreate(vertexId, key => ShortestPathGraph.Build(Graph, vertexId));
        }
    }
}