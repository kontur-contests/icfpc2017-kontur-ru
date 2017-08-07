using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Bingo.Graph;
using FluentAssertions;
using NUnit.Framework;
#pragma warning disable 618

namespace lib.GraphImpl
{
    public class Dinic2
    {
        private static int MAXN = 50000;
        public static int INF = (int)1e9;

        private int nodesCount, s, t;
        int[] deepness = new int[MAXN];
        int[] ptr = new int[MAXN];
        int[] queue = new int[MAXN];
        List<Edge2> edges = new List<Edge2>();
        List<int>[] edgeLists = new List<int>[MAXN];
        Dictionary<int, int> vertexesIndexes = new Dictionary<int, int>();
        Dictionary<int, int> realIndexes = new Dictionary<int, int>();

        public List<Edge> GetMinCut()
        {
            var result = new List<Edge>();

            for (int v = 0; v < nodesCount; v++)
            {
                if (deepness[v] == -1)
                    continue;
                foreach (var edge in edgeLists[v])
                {
                    int u = edges[edge].b;
                    if (deepness[u] == -1 && edges[edge].flow > 0)
                        result.Add(new Edge(realIndexes[v], realIndexes[u], -1, -1));
                }
            }
            return result;
        }


        int[] components = new int[MAXN];
        public List<Edge> GetOptimalMinCut(int compSize, out int size)
        {
            var componentsList = new List<ComponentNode> { null };

            for (int i = 0; i < nodesCount && edgeLists[i] != null; i++)
            {
                if (components[i] != 0)
                    continue;
                var component = new ComponentNode { ComponentId = componentsList.Count };
                var queue = new Queue<int>();
                components[i] = component.ComponentId;
                queue.Enqueue(i);

                while (queue.Count > 0)
                {
                    var node = queue.Dequeue();
                    for (var index = 0; index < edgeLists[node].Count; index++)
                    {
                        var nextEdge = edgeLists[node][index];
                        var to = edges[nextEdge].b;

                        if(components[to] == component.ComponentId)
                            continue;

                        if (components[to] != 0)
                        {
                            component.Edges.Add(components[to]);
                            componentsList[components[to]].Edges.Add(component.ComponentId);
                            continue;
                        }
                        if(edges[nextEdge].flow == edges[nextEdge].cap)
                            continue;

                        components[to] = component.ComponentId;
                        component.VerticesCount++;
                        queue.Enqueue(to);
                    }


                }
                componentsList.Add(component);
            }

            var compQueue = new Queue<ComponentNode>();

            var startComponent = componentsList[components[s]];
            var endComponent = componentsList[components[t]];
            var usedComponents = new HashSet<int> { startComponent.ComponentId };
            var ourComponents = new HashSet<int> { startComponent.ComponentId };
            var ourComponentSize = startComponent.VerticesCount;
            compQueue.Enqueue(startComponent);
            while (compQueue.Count > 0)
            {
                var com = compQueue.Dequeue();
                foreach (var edge in com.Edges)
                {
                    if(edge == endComponent.ComponentId || usedComponents.Contains(edge))
                        continue;
                    usedComponents.Add(edge);
                    var nextComponent = componentsList[edge];
                    if(ourComponentSize + nextComponent.VerticesCount > compSize / 2)
                        continue;
                    ourComponents.Add(nextComponent.ComponentId);
                    ourComponentSize += nextComponent.VerticesCount;
                    compQueue.Enqueue(nextComponent);
                }
            }

            var result = new List<Edge>();

            for (int v = 0; v < nodesCount && edgeLists[v] != null; v++)
            {
                foreach (var edge in edgeLists[v])
                {
                    int u = edges[edge].b;
                    if (ourComponents.Contains(components[v]) && !ourComponents.Contains(components[u]))
                        result.Add(new Edge(realIndexes[v], realIndexes[u], -1, -1));
                }
            }

            size = ourComponentSize;

            return result;
        }

        private class ComponentNode
        {
            public int ComponentId;
            public int VerticesCount;
            public HashSet<int> Edges = new HashSet<int>();
        }


        int GetOrCreateIndex(int x)
        {
            if (!vertexesIndexes.ContainsKey(x))
            {
                int ind = vertexesIndexes.Count;
                vertexesIndexes[x] = ind;
                realIndexes[ind] = x;
                edgeLists[ind] = new List<int>();
            }
            return vertexesIndexes[x];
        }

        void add_edge(int a, int b, int cap)
        {
            a = GetOrCreateIndex(a);
            b = GetOrCreateIndex(b);

            Edge2 e1 = new Edge2(a, b, cap, 0);
            Edge2 e2 = new Edge2(b, a, 0, 0);
            edgeLists[a].Add(edges.Count);
            edges.Add(e1);
            edgeLists[b].Add(edges.Count);
            edges.Add(e2);
        }

        bool bfs()
        {
            int queueHead = 0, queueTail = 0;
            queue[queueTail++] = s;
            deepness = Enumerable.Repeat(-1, nodesCount).ToArray();
            deepness[s] = 0;
            while (queueHead < queueTail && deepness[t] == -1)
            {
                int nextVerex = queue[queueHead++];
                for (int nextEdge = 0; nextEdge < edgeLists[nextVerex].Count; ++nextEdge)
                {
                    int nextVertexId = edgeLists[nextVerex][nextEdge],
                        to = edges[nextVertexId].b;
                    if (deepness[to] == -1 && edges[nextVertexId].flow < edges[nextVertexId].cap)
                    {
                        queue[queueTail++] = to;
                        deepness[to] = deepness[nextVerex] + 1;
                    }
                }
            }
            return deepness[t] != -1;
        }

        int dfs(int vertex, int flow)
        {
            if (flow == 0) return 0;
            if (vertex == t) return flow;
            for (; ptr[vertex] < (int)edgeLists[vertex].Count; ++ptr[vertex])
            {
                int id = edgeLists[vertex][ptr[vertex]],
                    to = edges[id].b;
                if (deepness[to] != deepness[vertex] + 1) continue;
                int pushed = dfs(to, Math.Min(flow, edges[id].cap - edges[id].flow));
                if (pushed > 0)
                {
                    edges[id].flow += pushed;
                    edges[id ^ 1].flow -= pushed;
                    return pushed;
                }
            }
            return 0;
        }

        public Dinic2(Graph graph, int punterId, int s, int t, out int flow, bool expandST = false)
        {
            if (s == t) throw new ArgumentException($"s == t == {s}");
            if (!expandST)
            {
                s = GetOrCreateIndex(s);
                t = GetOrCreateIndex(t);
                nodesCount = graph.Vertexes.Count;
            }
            else
            {
                int ss = 100500 + s;
                int tt = 100500 + t;
                foreach (var edge in graph.Vertexes[s].Edges)
                    add_edge(ss, edge.To, INF);
                foreach (var edge in graph.Vertexes[t].Edges)
                    add_edge(edge.To, tt, INF);
                s = GetOrCreateIndex(ss);
                t = GetOrCreateIndex(tt);
                nodesCount = graph.Vertexes.Count + 2;
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
}