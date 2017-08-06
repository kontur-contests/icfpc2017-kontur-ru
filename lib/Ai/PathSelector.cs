using System;
using System.Collections.Generic;
using FluentAssertions;
using System.Linq;
using lib.GraphImpl;
using MoreLinq;
using NUnit.Framework;

namespace lib.Ai
{
    public class PathSelector
    {
        private readonly Map map;
        private Graph graph;
        private MineDistCalculator minDists;
        private readonly int length;

        public PathSelector(Map map, MineDistCalculator minDists, int length)
        {
            this.map = map;
            graph = new Graph(map);
            this.minDists = minDists;
            this.length = length;
        }

        [ThreadStatic]
        static Random rand;
        public List<int> SelectPath()
        {
            rand = rand ?? new Random();
            if (map.Mines.Length == 0) return new List<int>();
            if (map.Mines.Length == 1)
            {
                var mine = map.Mines[0];
                var candidates = map.Sites.Where(s => Math.Abs(minDists.GetDist(mine, s.Id) - length) < length/10).MaxListBy(f => graph.Vertexes[f.Id].Edges.Count);
                if (!candidates.Any())
                    return new List<int>();

                var future = candidates[rand.Next(candidates.Count)].Id;
                return minDists.GetReversedPath(mine, future).Reverse().ToList();
            }
            if (map.Mines.Length == 2)
            {
                var allPairs =
                    from m1 in map.Mines
                    from m2 in map.Mines
                    where m1 != m2
                    let p12 = minDists.GetReversedPath(m1, m2).Reverse().ToList()
                    select new { m1, m2, p12 };
                var closestTuple = allPairs.MinBy(t => t.p12.Count);
                return closestTuple.p12.Take(length + 1).ToList();
            }
            else
            {
                var minesPaths = map.Mines.Select(GreedyGrowPath).ToList();
                var bestMinesPath = minesPaths.MaxBy(EstimatePathByDinic);
                var fullPath = bestMinesPath
                    .Pairwise((a, b) => minDists.GetReversedPath(a, b).Reverse().Skip(1)).SelectMany(z => z)
                    .Take(length)
                    .ToList();
                fullPath.Insert(0, bestMinesPath[0]);
                return fullPath.ToList();
            }
        }

        private double EstimatePath(List<int> minesPath)
        {
            int firstLastDist = minDists.GetDist(minesPath.First(), minesPath.Last());
            return minesPath.Count + firstLastDist / length + minesPath.Min(m => graph.Vertexes[m].Edges.Count) / 100;
        }
        
        private double EstimatePathByDinic(List<int> minesPath)
        {
            return new Dinic(graph, 0, minesPath.First(), minesPath.Last(), out int flow).GetMinCut().Count * 100000 + EstimatePath(minesPath);
        }
        
        private List<int> GreedyGrowPath(int startMine)
        {
            List<int> path = new List<int>(){startMine};
            var mines = map.Mines.ToHashSet();
            mines.Remove(startMine);
            var totalLen = 0;
            while(totalLen < length && mines.Count > 0)
            {
                var prev = path.Last();
                int next = mines.MinBy(mNext => minDists.GetDist(mNext, prev));
                path.Add(next);
                mines.Remove(next);
                totalLen += minDists.GetDist(prev, next);
            }
            return path;
        }
    }

    [TestFixture]
    public class PathSelector_Should
    {
        [Test]
        [Explicit]
        public void SelectMines()
        {
            var map = MapLoader.LoadMapByNameInTests("tube.json").Map;
            var desiredLen = 15;
            var graph = new Graph(map);
            var minDists = new MineDistCalculator(graph);
            var path = new PathSelector(map, minDists, desiredLen).SelectPath();
            var positioner = new FuturesPositioner(map, graph, path, minDists);
            var futures = positioner.GetFutures();
            map.ShowWithPath(path, futures);
        }

        [Test]
        public void SelectMinesOnTube()
        {
            var map = MapLoader.LoadMapByNameInTests("tube.json").Map;
            var desiredLen = 12;
            var path = new PathSelector(map, new MineDistCalculator(new Graph(map)), desiredLen).SelectPath();
            Assert.AreEqual(desiredLen + 1, path.Count);
            path.Should().Contain(new[] { 285, 260, 148, 64 });
        }
    }
}