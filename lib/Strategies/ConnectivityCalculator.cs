using System;
using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using MoreLinq;
using NUnit.Framework;

namespace lib.Strategies
{
    public class CutSizeInfo
    {
        public int CutSize;
        public Vertex V1, V2;

        public CutSizeInfo(Vertex v1, Vertex v2, int cutSize)
        {
            V1 = v1;
            V2 = v2;
            CutSize = cutSize;
        }

        public override string ToString()
        {
            return $"{nameof(V1)}: {V1}, {nameof(V2)}: {V2}, {nameof(CutSize)}: {CutSize}";
        }
    }

    public class ConnectivityCalculator
    {
        private readonly Graph graph;

        public ConnectivityCalculator(Graph graph)
        {
            this.graph = graph;
        }

        public IEnumerable<CutSizeInfo> CutSizeForEachMinePair()
        {
            return
                from m1 in graph.Mines.Values
                from m2 in graph.Mines.Values
                where m1 != m2
                let cutSize = GetCutSize(m1, m2)
                select new CutSizeInfo(m1, m2, cutSize);
        }

        private int GetCutSize(Vertex v1, Vertex v2)
        {
            var dinic = new Dinic(graph, 0, v1.Id, v2.Id, out int flow, true);
            return flow;
        }
    }

    [TestFixture]
    public class Connectivity_Should
    {
        [Test, Explicit]
        public void DoSomething_WhenSomething()
        {
            foreach (var map in MapLoader.LoadOnlineMaps())
            {
                var graph = new Graph(map.Map);
                var en = new ConnectivityCalculator(graph).CutSizeForEachMinePair().OrderBy(s => s.CutSize).ToList();
                IList<int> sizes = en.Select(s => s.CutSize).ToList();
                //Console.WriteLine(sizes.Reverse().Take(10).ToDelimitedString(","));
                Console.WriteLine();
                Console.WriteLine(map.Name + " " + (en.Count > 0 ? en.Average(c => c.CutSize) : -1));
                Console.WriteLine(sizes.ToDelimitedString(","));
            }
        }
    }
}