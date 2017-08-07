using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
#pragma warning disable 618

namespace lib.GraphImpl
{
    public class Dinic
    {
        private static int MAXN = 5000;
        public static int INF = (int)1e9;

        private int n, s, t;
        int[] d = new int[MAXN];
        int[] ptr = new int[MAXN];
        int[] q = new int[MAXN];
        List<Edge2> e = new List<Edge2>();
        List<int>[] g = new List<int>[MAXN];
        Dictionary<int, int> inds = new Dictionary<int, int>();
        Dictionary<int, int> rinds = new Dictionary<int, int>();

        public List<Edge> GetMinCut()
        {
            var result = new List<Edge>();

            for (int v = 0; v < n; v++)
            {
                if (d[v] == -1)
                    continue;
                foreach (var ee in g[v])
                {
                    int u = e[ee].b;
                    if (d[u] == -1 && e[ee].flow > 0)
                        result.Add(new Edge(rinds[v], rinds[u], -1, -1));
                }
            }
            return result;
        }

        int v(int x)
        {
            if (!inds.ContainsKey(x))
            {
                int ind = inds.Count;
                inds[x] = ind;
                rinds[ind] = x;
                g[ind] = new List<int>();
            }
            return inds[x];
        }

        void add_edge(int a, int b, int cap)
        {
            a = v(a);
            b = v(b);

            Edge2 e1 = new Edge2(a, b, cap, 0);
            Edge2 e2 = new Edge2(b, a, 0, 0);
            g[a].Add(e.Count);
            e.Add(e1);
            g[b].Add(e.Count);
            e.Add(e2);
        }

        bool bfs()
        {
            int qh = 0, qt = 0;
            q[qt++] = s;
            d = Enumerable.Repeat(-1, n).ToArray();
            d[s] = 0;
            while (qh < qt && d[t] == -1)
            {
                int v = q[qh++];
                for (int i = 0; i < g[v].Count; ++i)
                {
                    int id = g[v][i],
                        to = e[id].b;
                    if (d[to] == -1 && e[id].flow < e[id].cap)
                    {
                        q[qt++] = to;
                        d[to] = d[v] + 1;
                    }
                }
            }
            return d[t] != -1;
        }

        int dfs(int v, int flow)
        {
            if (flow == 0) return 0;
            if (v == t) return flow;
            for (; ptr[v] < (int)g[v].Count; ++ptr[v])
            {
                int id = g[v][ptr[v]],
                    to = e[id].b;
                if (d[to] != d[v] + 1) continue;
                int pushed = dfs(to, Math.Min(flow, e[id].cap - e[id].flow));
                if (pushed > 0)
                {
                    e[id].flow += pushed;
                    e[id ^ 1].flow -= pushed;
                    return pushed;
                }
            }
            return 0;
        }

        public Dinic(Graph graph, int punterId, int s, int t, out int flow, bool expandST = false)
        {
            if (s == t) throw new ArgumentException($"s == t == {s}");
            if (!expandST)
            {
                s = v(s);
                t = v(t);
                n = graph.Vertexes.Count;
            }
            else
            {
                int ss = 100500 + s;
                int tt = 100500 + t;
                foreach (var edge in graph.Vertexes[s].Edges)
                    add_edge(ss, edge.To, INF);
                foreach (var edge in graph.Vertexes[t].Edges)
                    add_edge(edge.To, tt, INF);
                s = v(ss);
                t = v(tt);
                n = graph.Vertexes.Count + 2;
            }
            this.s = s;
            this.t = t;


            foreach (var vertex in graph.Vertexes)
            {
                foreach (var edge in vertex.Value.Edges)
                {
                    if (edge.Owner != -1)// && edge.Owner != punterId)
                        continue;

                    int cap = edge.Owner == punterId
                        ? INF
                        : 1;

                    add_edge(edge.From, edge.To, cap);
                }
            }

            flow = 0;
            for (;;)
            {
                if (!bfs()) break;
                ptr = new int[MAXN];
                int pushed;
                while ((pushed = dfs(s, INF)) > 0)
                    flow += pushed;
            }
        }

        class Edge2
        {
            public int a, b, cap, flow;

            public Edge2(int a, int b, int cap, int flow)
            {
                this.a = a;
                this.b = b;
                this.cap = cap;
                this.flow = flow;
            }

            public override string ToString()
            {
                return $"{nameof(a)}: {a}, {nameof(b)}: {b}, {nameof(cap)}: {cap}, {nameof(flow)}: {flow}";
            }
        }
    }

    [TestFixture]
    public class Denic_Test
    {
        [Test]
        public void Test()
        {
            var graph = new Graph();

            graph.AddVertex(0);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);

            var me = 13;
            graph.AddEdge(0, 1);
            graph.AddEdge(0, 2);
            graph.AddEdge(0, 3);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 4);
            graph.AddEdge(3, 4);

            var dinic = new Dinic(graph, me, 0, 4, out int flow);
            flow.ShouldBeEquivalentTo(2);

            var cut = dinic.GetMinCut();
            cut.ShouldBeEquivalentTo(new List<Edge>
            {
                new Edge(0, 3, -1, -1),
                new Edge(2, 4, -1, -1),

            });
        }
    }
}