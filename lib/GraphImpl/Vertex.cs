using System.Collections.Generic;

namespace lib.GraphImpl
{
    public class Vertex
    {
        public readonly int Id;
        public readonly bool IsMine;
        public readonly List<Edge> Edges = new List<Edge>();

        public override string ToString()
        {
            var name = IsMine ? "Mine" : "Vertex";
            return $"{name} {Id} deg = {Edges.Count}";
        }

        public Vertex(int id, bool isMine)
        {
            Id = id;
            IsMine = isMine;
        }
    }
}