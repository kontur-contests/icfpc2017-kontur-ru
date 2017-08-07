using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace lib.GraphImpl
{
    public class Graph_Test : TestBase
    {
        [Test, Ignore("broken")]
        public void Test()
        {
            var map = new Map(
                new[] {new Site(1, 0, 0), new Site(2, 0, 0), new Site(3, 0, 0)},
                new[] {new River(1, 2), new River(2, 3, 1)},
                new[] {2});
            var graph = new Graph(map);
            graph.Vertexes.ShouldBeEquivalentTo(
                new Dictionary<int, Vertex>
                {
                    {
                        1, new Vertex(1, false)
                        {
                            Edges = {new Edge(1, 2, -1, -1)}
                        }
                    },
                    {
                        2, new Vertex(2, true)
                        {
                            Edges = {new Edge(2, 1, -1, -1), new Edge(2, 3, 1, -1)}
                        }
                    },
                    {
                        3, new Vertex(3, false)
                        {
                            Edges = {new Edge(3, 2, 1, -1)}
                        }
                    }
                });
        }
    }
}