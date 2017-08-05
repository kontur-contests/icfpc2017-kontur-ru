using System.Collections.Generic;

namespace lib.GraphImpl
{
    public class Graph
    {
        public readonly Dictionary<int, Vertex> Vertexes = new Dictionary<int, Vertex>();
        public readonly Dictionary<int, Vertex> Mines = new Dictionary<int, Vertex>();

        public Graph()
        {
            
        }

        public Graph(Map map)
        {
            var mineIds = new HashSet<int>(map.Mines);
            foreach (var site in map.Sites)
            {
                var vertex = new Vertex(site.Id, mineIds.Contains(site.Id));
                Vertexes.Add(vertex.Id, vertex);
                if (vertex.IsMine)
                    Mines.Add(vertex.Id, vertex);
            }
            foreach (var river in map.Rivers)
            {
                Vertexes[river.Source].Edges.Add(Edge.Forward(river));
                if (river.Source != river.Target)
                    Vertexes[river.Target].Edges.Add(Edge.Forward(river));
            }
        }

        public void AddVertex(int v, bool isMine = false)
        {
            if (Vertexes.ContainsKey(v))
                return;
            Vertexes.Add(v, new Vertex(v, isMine));
            if (isMine)
                Mines.Add(v, new Vertex(v, true));
        }

        public void AddEdge(int v, int u, int owner = -1)
        {
            var river = new River(v, u, owner);
            Vertexes[v].Edges.Add(Edge.Forward(river));
            Vertexes[u].Edges.Add(Edge.Backward(river));
        }
    }
}