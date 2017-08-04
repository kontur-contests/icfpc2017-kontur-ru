using System;
using System.Collections.Generic;
using System.Linq;

namespace lib.GraphImpl
{
    public class Graph
    {
        public readonly Dictionary<int, Vertex> vertexes = new Dictionary<int, Vertex>();

        public Graph()
        {
            
        }

        public Graph(Map map)
        {
            var mineIds = new HashSet<int>(map.Mines);
            foreach (var site in map.Sites)
                vertexes.Add(site.Id, new Vertex(site.Id, mineIds.Contains(site.Id)));
            foreach (var river in map.Rivers)
            {
                vertexes[river.Source].Edges.Add(new Edge(river.Target, river.Owner));
                if (river.Source != river.Target)
                    vertexes[river.Target].Edges.Add(new Edge(river.Source, river.Owner));
            }
        }

        public void AddVertex(int v, bool isMine = false)
        {
            if (vertexes.ContainsKey(v))
                return;
            vertexes.Add(v, new Vertex(v, isMine));
        }

        public void AddEdge(int v, int u)
        {
            vertexes[v].Edges.Add(new Edge(u, -1));
            vertexes[u].Edges.Add(new Edge(v, -1));
        }
    }
}