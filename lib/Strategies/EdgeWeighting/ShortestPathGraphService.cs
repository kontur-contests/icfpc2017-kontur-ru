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
        private IDictionary<Tuple<int, int>, ShortestPathGraph> ComponentsCache { get; } = new Dictionary<Tuple<int, int>, ShortestPathGraph>();
        private IDictionary<int, ShortestPathGraph> VertexesCache { get; } = new Dictionary<int, ShortestPathGraph>();

        public ShortestPathGraph ForComponent(ConnectedComponent component, Dictionary<int, ConnectedComponent> vertexComponent, int punter)
        {
            return ComponentsCache.GetOrCreate(
                Tuple.Create(component.OwnerPunterId, component.Id), key => ShortestPathGraph.Build(Graph, edge =>
                {
                    if (edge.Owner == -1)
                        return true;
                    if (edge.Owner != punter) // Options (�� ������� ��� ��� ��������� Options)
                        return false;
                    if (vertexComponent.ContainsKey(edge.To) && vertexComponent.ContainsKey(edge.From) && vertexComponent[edge.To] == vertexComponent[edge.From])
                        return false;
                    return true;
                }, component.Vertices));
        }

        public ShortestPathGraph ForVertex(int vertexId)
        {
            return VertexesCache.GetOrCreate(vertexId, key => ShortestPathGraph.Build(Graph, vertexId));
        }
    }
}