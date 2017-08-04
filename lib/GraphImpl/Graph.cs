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
                Vertexes[river.Source].Edges.Add(new Edge(river.Source, river.Target, river.Owner));
                if (river.Source != river.Target)
                    Vertexes[river.Target].Edges.Add(new Edge(river.Target, river.Source, river.Owner));
            }
        }

        public void AddVertex(int v, bool isMine = false)
        {
            if (Vertexes.ContainsKey(v))
                return;
            Vertexes.Add(v, new Vertex(v, isMine));
        }

        public void AddEdge(int v, int u)
        {
            Vertexes[v].Edges.Add(new Edge(v, u, -1));
            Vertexes[u].Edges.Add(new Edge(u, v, -1));
        }
    }
}