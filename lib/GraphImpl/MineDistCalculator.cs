using System;
using System.Collections.Generic;
using System.Linq;
using lib.StateImpl;
using NUnit.Framework;

namespace lib.GraphImpl
{
    public class MineDistCalculator : IService
    {
        public Impl impl;

        public class ServiceState
        {
            public Dictionary<int, Dictionary<int, int[]>> distFromMines;
        }

        public class Impl
        {
            private readonly Graph graph;
            public readonly Dictionary<int, Dictionary<int, SingleLinkedList<int>>> distFromMines;

            public Impl(Graph graph, Dictionary<int, Dictionary<int, int[]>> distFromMines)
            {
                this.graph = graph;
                this.distFromMines = distFromMines.ToDictionary(x => x.Key, x => x.Value.ToDictionary(t => t.Key, t => new SingleLinkedList<int>(t.Value)));
            }

            public Impl(Graph graph)
            {
                this.graph = graph;

                distFromMines = new Dictionary<int, Dictionary<int, SingleLinkedList<int>>>();
                foreach (var vertex in graph.Vertexes.Values)
                {
                    if (vertex.IsMine)
                    {
                        var dists = CalcDist(vertex.Id);
                        distFromMines.Add(vertex.Id, dists);
                    }
                }
            }

            public int GetDist(int mineId, int vertexId)
            {
                if (!distFromMines.ContainsKey(mineId))
                    throw new InvalidOperationException();
                if (!distFromMines[mineId].ContainsKey(vertexId))
                    return -1;
                return distFromMines[mineId][vertexId].Length - 1;
            }

            public SingleLinkedList<int> GetPath(int mineId, int vertexId)
            {
                if (!distFromMines.ContainsKey(mineId))
                    throw new InvalidOperationException();
                if (!distFromMines[mineId].ContainsKey(vertexId))
                    return null;
                return distFromMines[mineId][vertexId];
            }

            //(v, dist)

            private Dictionary<int, SingleLinkedList<int>> CalcDist(int start)
            {
                var dist = new Dictionary<int, SingleLinkedList<int>>();
                var queue = new Queue<int>();

                dist[start] = new SingleLinkedList<int>(start);
                queue.Enqueue(start);

                while (queue.Any())
                {
                    int v = queue.Dequeue();
                    foreach (var edge in graph.Vertexes[v].Edges)
                    {
                        int u = edge.To;
                        if (dist.ContainsKey(u))
                            continue;
                        dist.Add(u, new SingleLinkedList<int>(u, dist[v]));
                        queue.Enqueue(u);
                    }
                }

                return dist;
            }
        }

        public void Setup(State state, IServices services)
        {
            var graph = services.Get<GraphService>(state).Graph;
            impl = new Impl(graph);
            state.mdc = new ServiceState { distFromMines = impl.distFromMines.ToDictionary(x => x.Key, x => x.Value.ToDictionary(t => t.Key, t => t.Value.Enumerate().ToArray())) };
        }

        public void ApplyNextState(State state, IServices services)
        {
            var graph = services.Get<GraphService>(state).Graph;
            impl = impl ?? new Impl(graph, state.mdc.distFromMines);
        }

        public int GetDist(int mineId, int vertexId)
        {
            return impl.GetDist(mineId, vertexId);
        }

        public SingleLinkedList<int> GetPath(int mineId, int vertexId)
        {
            return impl.GetPath(mineId, vertexId);
        }
    }

    public class SingleLinkedList<T>
    {
        public SingleLinkedList(T[] values, int i = 0)
        {
            Value = values[i];
            Length = values.Length - i;
            if (i + 1 < values.Length)
                Prev = new SingleLinkedList<T>(values, i + 1);
        }

        public SingleLinkedList(T value)
        {
            Value = value;
            Length = 1;
        }

        public SingleLinkedList(T value, SingleLinkedList<T> prev)
        {
            Value = value;
            Prev = prev;
            Length = prev.Length + 1;
        }

        public IEnumerable<T> Enumerate()
        {
            yield return Value;
            if (Prev == null) yield break;
            foreach (var v in Prev.Enumerate())
            {
                yield return v;
            }
        }

        public T Value;
        public int Length;
        public SingleLinkedList<T> Prev;
    }

    [TestFixture]
    public class MineDistCalculator_Tests
    {
        [Test]
        public void TestDisjoint()
        {
            var graph = new Graph();

            graph.AddVertex(1, true);
            graph.AddVertex(2);

            var calculator = new MineDistCalculator.Impl(graph);
            Assert.AreEqual(-1, calculator.GetDist(1, 2));
        }

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

            var calculator = new MineDistCalculator.Impl(graph);
            
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