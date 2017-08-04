using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace lib.GraphImpl
{
    public class ConnectedCalculator
    {
        private readonly Graph graph;
        private readonly Dictionary<int, HashSet<int>> minesConnected;

        public ConnectedCalculator(Graph graph, int ownerId)
        {
            this.graph = graph;

            minesConnected = new Dictionary<int, HashSet<int>>();
            foreach (var vertex in graph.Mines.Values)
            {
                Bfs(vertex.Id, ownerId);
            }
        }

        public HashSet<int> GetConnectedMines(int v)
        {
            return minesConnected.ContainsKey(v)
                ? minesConnected[v]
                : new HashSet<int>();
        }

        private void Bfs(int start, int owner)
        {
            var dist = new Dictionary<int, int>();
            var queue = new Queue<int>();

            dist[start] = 0;
            queue.Enqueue(start);

            while (queue.Any())
            {
                int v = queue.Dequeue();
                foreach (var edge in graph.Vertexes[v].Edges)
                {
                    if (edge.Owner != owner)
                        continue;

                    int u = edge.To;
                    if (dist.ContainsKey(u))
                        continue;
                    dist.Add(u, dist[v] + 1);
                    queue.Enqueue(u);
                }
            }

            foreach (var v in dist.Keys)
            {
                if (!minesConnected.ContainsKey(v))
                    minesConnected.Add(v, new HashSet<int>());
                minesConnected[v].Add(start);
            }
        }
    }

    [TestFixture]
    public class ConnectedCalculator_Test
    {
        [Test]
        public void TestGraphWithTwoMines()
        {
            var graph = new Graph();

            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3, true);
            graph.AddVertex(4, true);
            graph.AddVertex(5);
            graph.AddVertex(6, true);

            var me = 13;
            graph.AddEdge(1, 4, me);
            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3, me);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 5);
            graph.AddEdge(5, 6, me);

            var calculator = new ConnectedCalculator(graph, me);

            calculator.GetConnectedMines(1).ShouldBeEquivalentTo(
                new List<int> {3, 4});
            calculator.GetConnectedMines(2).ShouldBeEquivalentTo(
                new List<int> {  });
            calculator.GetConnectedMines(3).ShouldBeEquivalentTo(
                new List<int> { 3, 4 });
            calculator.GetConnectedMines(4).ShouldBeEquivalentTo(
                new List<int> { 3, 4 });
            calculator.GetConnectedMines(5).ShouldBeEquivalentTo(
                new List<int> { 6 });
            calculator.GetConnectedMines(6).ShouldBeEquivalentTo(
                new List<int> { 6 });
        }
    }
}