using System.Collections.Generic;

namespace lib.GraphImpl.ShortestPath
{
    public class ShortestPathVertex
    {
        public ShortestPathVertex(int id, int distance)
        {
            Id = id;
            Distance = distance;
        }

        public int Id { get; }
        public int Distance { get; }

        public List<Edge> Edges { get; } = new List<Edge>();

        /// <summary>
        ///     Edges that lead to a vertex with the same <see cref="Distance" />
        /// </summary>
        public List<Edge> SameLayerEdges { get; } = new List<Edge>();
    }
}