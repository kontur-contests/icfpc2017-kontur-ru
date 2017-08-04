using System.Collections.Generic;

namespace lib.GraphImpl
{
    public class Graph
    {
        public readonly Dictionary<int, Vertex> Vertexes = new Dictionary<int, Vertex>();

        public Graph(Map map)
        {
            var mineIds = new HashSet<int>(map.Mines);
            foreach (var site in map.Sites)
                Vertexes.Add(site.Id, new Vertex(site.Id, mineIds.Contains(site.Id)));
            foreach (var river in map.Rivers)
            {
                Vertexes[river.Source].Edges.Add(new Edge(river.Target, river.Owner));
                if (river.Source != river.Target)
                    Vertexes[river.Target].Edges.Add(new Edge(river.Source, river.Owner));
            }
        }
    }
}