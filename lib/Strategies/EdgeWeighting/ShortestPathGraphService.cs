using System;
using System.Collections.Generic;
using lib.GraphImpl;
using lib.GraphImpl.ShortestPath;
using lib.StateImpl;

namespace lib.Strategies.EdgeWeighting
{
    public class ShortestPathGraphService : IService
    {
        public ShortestPathGraphService(Graph graph, ConnectedComponentsService connectedComponentsService)
        {
            Graph = graph;
            ConnectedComponentsService = connectedComponentsService;
        }

        private Graph Graph { get; set; }
        private ConnectedComponentsService ConnectedComponentsService { get; set; }

        private IDictionary<Tuple<int, int, int>, ShortestPathGraph> ComponentsCache { get; } =
            new Dictionary<Tuple<int, int, int>, ShortestPathGraph>();

        public ShortestPathGraph ForComponent(ConnectedComponent component, Dictionary<int, ConnectedComponent> vertexComponent, int optionsCount)
        {
            return ComponentsCache.GetOrCreate(
                Tuple.Create(component.OwnerPunterId, component.Id, optionsCount), key => optionsCount <= 0
                    ? BuildShortestPathGraph(component, vertexComponent)
                    : BuildShortestPathGraphWithOptions(component, vertexComponent, optionsCount));
        }

        private ShortestPathGraphWithOptions BuildShortestPathGraphWithOptions(ConnectedComponent component, Dictionary<int, ConnectedComponent> vertexComponent, int optionsCount)
        {
            return ShortestPathGraphWithOptions.Build(
                Graph, edge =>
                {
                    if (!edge.IsOwnedBy(component.OwnerPunterId) && !edge.CanBeOwnedBy(component.OwnerPunterId, true))
                        return false;
                    if (vertexComponent.ContainsKey(edge.To) && vertexComponent.ContainsKey(edge.From) &&
                        vertexComponent[edge.To] == vertexComponent[edge.From])
                        return false;
                    return true;
                }, component.Vertices, optionsCount);
        }

        private ShortestPathGraph BuildShortestPathGraph(ConnectedComponent component, Dictionary<int, ConnectedComponent> vertexComponent)
        {
            return ShortestPathGraph.Build(
                Graph, edge =>
                {
                    if (edge.Owner == -1)
                        return true;
                    if (!edge.IsOwnedBy(component.OwnerPunterId))
                        return false;
                    if (vertexComponent.ContainsKey(edge.To) && vertexComponent.ContainsKey(edge.From) &&
                        vertexComponent[edge.To] == vertexComponent[edge.From])
                        return false;
                    return true;
                }, component.Vertices);
        }
    }
}