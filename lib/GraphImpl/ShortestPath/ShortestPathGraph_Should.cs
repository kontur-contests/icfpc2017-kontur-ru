using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace lib.GraphImpl.ShortestPath
{
    public class ShortestPathGraph_Should : TestBase
    {
        private class TestGraph : Graph, IEnumerable<int>
        {
            public TestGraph(params int[] vertices)
            {
                foreach (var vertex in vertices)
                    AddVertex(vertex);
            }

            public IEnumerator<int> GetEnumerator() => Vertexes.Values.Select(x => x.Id).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void Add(int vertexId, int[] neighborIds = null)
            {
                foreach (var neighborId in neighborIds ?? new int[0])
                    AddEdge(vertexId, neighborId);
            }
        }

        private class TestShortestPathGraph : ShortestPathGraph, IEnumerable<int>
        {
            public IEnumerator<int> GetEnumerator() => Vertexes.Values.OrderBy(x => x.Id).Select(x => x.Id).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void Add(int vertexId, int distance, int[] neighborIds = null, int[] sameLevelNeighborIds = null)
            {
                AddVertex(vertexId, distance);
                foreach (var neighborId in neighborIds ?? new int[0])
                    AddEdge(Edge.Forward(new River(vertexId, neighborId)));
                foreach (var neighborId in sameLevelNeighborIds ?? new int[0])
                    AddSameLayerEdge(Edge.Forward(new River(neighborId, vertexId)));
            }
        }

        [Test]
        public void GraphWithOptions()
        {
            var g = new TestGraph(Enumerable.Range(1, 6).ToArray())
            {
                {1, new[] {2}},
                {2, new[] {4}},
                {3, new[] {4, 6}},
                {4},
                {5, new[] {6}},
                {6},
            };

            g.AddEdge(2, 3, 1);
            g.AddEdge(3, 5, 1);

            var spGraph = ShortestPathGraphWithOptions.Build(g, edge => edge.IsFree || edge.MustUseOption, new[] {1}, 1);
            var testShortestPathGraph = new TestShortestPathGraph
            {
                {1, 0, new[] {2}},
                {2, 1, new[] {4, 3}},
                {3, 2, new[] {5, 6}},
                {4, 2, null, new[] {3}},
                {5, 4},
                {6, 3, new[] {5}},
            };
            testShortestPathGraph[2].Edges.Single(e => e.To == 3).River.Owner = 1;
            testShortestPathGraph[3].Edges.Single(e => e.To == 5).River.Owner = 1;
            spGraph.ShouldBeEquivalentTo(testShortestPathGraph);
        }

        [Test]
        public void Simple()
        {
            var g = new TestGraph(1, 2, 3, 4)
            {
                {1, new[] {2, 3}},
                {2, new[] {3, 4}},
                {3, new[] {4}},
                {4}
            };

            var spGraph = ShortestPathGraph.Build(g, edge => edge.Owner == -1, new[] {1});
            spGraph.ShouldBeEquivalentTo(
                new TestShortestPathGraph
                {
                    {1, 0, new[] {2, 3}},
                    {2, 1, new[] {4}},
                    {3, 1, new[] {4}, new[] {2}},
                    {4, 2}
                });
        }

        [Test]
        public void SimpleTwoSources()
        {
            var g = new TestGraph(1, 2, 3, 4)
            {
                {1, new[] {2, 3}},
                {2, new[] {3, 4}},
                {3, new[] {4}},
                {4}
            };

            var spGraph = ShortestPathGraph.Build(g, edge => edge.Owner == -1, new[] {1, 4});
            spGraph.ShouldBeEquivalentTo(
                new TestShortestPathGraph
                {
                    {1, 0, new[] {2, 3}},
                    {2, 1},
                    {3, 1, null, new[] {2}},
                    {4, 0, new[] {2, 3}}
                });
        }

        [Test]
        public void TriangularGrid()
        {
            var g = new TestGraph(Enumerable.Range(1, 10).ToArray())
            {
                {1, new[] {2, 3}},
                {2, new[] {3, 4, 5}},
                {3, new[] {5, 6}},
                {4, new[] {5, 7, 8}},
                {5, new[] {6, 8, 9}},
                {6, new[] {9, 10}},
                {7, new[] {8}},
                {8, new[] {9}},
                {9, new[] {10}},
                {10},
            };

            var spGraph = ShortestPathGraph.Build(g, edge => edge.Owner == -1, new[] {1});
            spGraph.ShouldBeEquivalentTo(
                new TestShortestPathGraph
                {
                    {1, 0, new[] {2, 3}},
                    {2, 1, new[] {4, 5}},
                    {3, 1, new[] {5, 6}, new[] {2}},
                    {4, 2, new[] {7, 8}},
                    {5, 2, new[] {8, 9}, new[] {4}},
                    {6, 2, new[] {9, 10}, new[] {5}},
                    {7, 3},
                    {8, 3, null, new[] {7}},
                    {9, 3, null, new[] {8}},
                    {10, 3, null, new[] {9}},
                });
        }
    }
}