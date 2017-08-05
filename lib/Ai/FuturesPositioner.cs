using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using lib.Structures;
using MoreLinq;
using NUnit.Framework;

namespace lib.Ai
{
    public class FuturesPositioner
    {
        private readonly Map map;
        private readonly Graph graph;
        private readonly MineDistCalculator minDists;
        private readonly IList<int> path;

        public FuturesPositioner(Map map, Graph graph, IList<int> path, MineDistCalculator minDists)
        {
            this.map = map;
            this.graph = graph;
            this.path = path;
            this.minDists = minDists;
        }

        public Future[] GetFutures()
        {
            var minesOnPath = path.Where(id => map.Mines.Contains(id)).ToList();
            var nonMinesOnPath = path.Where(id => !map.Mines.Contains(id)).ToList();
            return minesOnPath
                .Select(
                    mine => new Future(
                        mine,
                        nonMinesOnPath.MaxBy(site => minDists.GetDist(mine, site) + graph.Vertexes[site].Edges.Count / 10)))
                .ToArray();
        }
    }

    [TestFixture]
    public class FuturesPositioner_Should
    {
        [Test]
        public void DoSomething_WhenSomething()
        {
        }
    }
}