using System;
using System.Collections.Generic;
using lib.GraphImpl;
using lib.GraphImpl.ShortestPath;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class ShortestPathGraphService : IService
    {
        private Graph graph;

        private IDictionary<Tuple<int, int>, ShortestPathGraph> Cache { get; } = new Dictionary<Tuple<int, int>, ShortestPathGraph>();

        public void Setup(State state, IServices services)
        {
            services.Setup<GraphService>(state);
        }

        public void ApplyNextState(State state, IServices services)
        {
            graph = services.Get<GraphService>(state).Graph;
        }

        public ShortestPathGraph For(int punterId, int componentId)
        {
            return MoreExtensions.GetOrCreate(
                Cache, Tuple.Create(punterId, componentId), key =>
                {
                    var components = ConnectedComponent.GetComponents(graph, punterId);
                    return ShortestPathGraph.Build(graph, components[componentId].Vertices);
                });
        }
    }
}