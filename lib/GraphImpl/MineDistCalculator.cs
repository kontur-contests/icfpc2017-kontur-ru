using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace lib.GraphImpl
{
    public class MineDistCalculator
    {
        private readonly Graph graph;
        private readonly Dictionary<int, Dictionary<int, int>> distFromMines;

        public MineDistCalculator(Graph graph)
        {
            this.graph = graph;

            distFromMines = new Dictionary<int, Dictionary<int, int>>();
            foreach (var vertex in graph.Vertexes.Values)
            {
                var dists = CalcDist(vertex.Id);
                if (vertex.IsMine)
                    distFromMines.Add(vertex.Id, dists);
            }
        }

        public int GetDist(int mineId, int vertexId)
        {
            if (!distFromMines.ContainsKey(mineId))
                throw new InvalidOperationException();
            if (!distFromMines[mineId].ContainsKey(vertexId))
                return int.MaxValue;
            return distFromMines[mineId][vertexId];
        }

        //(v, dist)
        private Dictionary<int, int> CalcDist(int start)
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
                    int u = edge.To;
                    if (dist.ContainsKey(u))
                        continue;
                    dist.Add(u, dist[v] + 1);
                    queue.Enqueue(u);
                }
            }

            return dist;
        }
    }

    [TestFixture]
    public class MineDistCalculator_Tests
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

            graph.AddEdge(1, 4);
            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 5);

            var calculator = new MineDistCalculator(graph);
            
            Assert.AreEqual(1, calculator.GetDist(3, 1));
            Assert.AreEqual(1, calculator.GetDist(3, 2));
            Assert.AreEqual(0, calculator.GetDist(3, 3));
            Assert.AreEqual(2, calculator.GetDist(3, 4));
            Assert.AreEqual(1, calculator.GetDist(3, 5));

            Assert.AreEqual(1, calculator.GetDist(4, 1));
            Assert.AreEqual(2, calculator.GetDist(4, 2));
            Assert.AreEqual(2, calculator.GetDist(4, 3));
            Assert.AreEqual(0, calculator.GetDist(4, 4));
            Assert.AreEqual(3, calculator.GetDist(4, 5));
        }
    }
}