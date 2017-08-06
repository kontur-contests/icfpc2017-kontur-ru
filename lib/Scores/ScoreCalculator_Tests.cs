using System.Collections.Generic;
using System.Linq;
using lib.Scores.Simple;
using lib.Structures;
using NUnit.Framework;

namespace lib.Scores
{
    [TestFixture]
    public class ScoreCalculator_Tests
    {
        private const int PUNTER_ID = 1;

        [TestCaseSource(nameof(GetTestCases))]
        public long GetScore(Map map, Future[] futures)
        {
            var calculator = new SimpleScoreCalculator();

            return calculator.GetScore(PUNTER_ID, map, futures);
        }

        private static IEnumerable<TestCaseData> GetTestCases()
        {
            yield return GetTestCase("empty map", new Map(Sites(0), new River[0], new int[0]), new Future[0], 0);

            yield return GetTestCase("map with 2 sites and left mine", new Map(Sites(2), new[] {MyRiver(0, 1)}, new[] {0}), new Future[0], 1);

            yield return GetTestCase("map with 2 sites and right mine", new Map(Sites(2), new[] {MyRiver(0, 1)}, new[] {1}), new Future[0], 1);

            yield return GetTestCase("map with 2 sites and right mine and fulfilled future", new Map(Sites(2), new[] { MyRiver(0, 1) }, new[] { 1 }), new[] { Future(1, 0) }, 2);

            yield return GetTestCase("map with 2 sites and right mine and non-fulfilled future", new Map(Sites(2), new[] { EnemyRiver(0, 1) }, new[] { 1 }), new[] { Future(1, 0) }, -1);

            yield return GetTestCase("map with 2 sites and both mines", new Map(Sites(2), new[] {MyRiver(0, 1)}, new[] {0, 1}), new Future[0], 2);

            yield return GetTestCase("non-cycle map with 3 sites and side mine", new Map(Sites(3), new[] { MyRiver(0, 1), MyRiver(1, 2) }, new[] { 0 }), new Future[0], 5);

            yield return GetTestCase("non-cycle map with 3 sites and side mine and fulfilled future", new Map(Sites(3), new[] { MyRiver(0, 1), MyRiver(1, 2) }, new[] { 0 }), new[] { Future(0, 2) }, 13);

            yield return GetTestCase("non-cycle map with 3 sites and side mine and non-fulfilled future", new Map(Sites(3), new[] { MyRiver(0, 1), EnemyRiver(1, 2) }, new[] { 0 }), new[] { Future(0, 2) }, -7);

            yield return GetTestCase("non-cycle map with 3 sites and center mine", new Map(Sites(3), new[] {MyRiver(0, 1), MyRiver(1, 2)}, new[] {1}), new Future[0], 2);

            yield return GetTestCase("cycle map with 3 sites", new Map(Sites(3), new[] {MyRiver(0, 1), MyRiver(1, 2), EnemyRiver(2, 0)}, new[] {0}), new Future[0], 2);

            var example1Rivers = new[]
            {
                EnemyRiver(0, 1),
                MyRiver(1, 2),
                MyRiver(2, 3),
                MyRiver(3, 4),
                EnemyRiver(4, 5),
                EnemyRiver(5, 6),
                EnemyRiver(6, 7),
                EnemyRiver(7, 0),
                EnemyRiver(7, 1),
                EnemyRiver(1, 3),
                EnemyRiver(3, 5),
                EnemyRiver(5, 7)
            };
            yield return GetTestCase("example1", new Map(Sites(8), example1Rivers, new[] {1, 5}), new Future[0], 6);

            var example2Rivers = new[]
            {
                EnemyRiver(0, 1),
                EnemyRiver(1, 2),
                EnemyRiver(2, 3),
                EnemyRiver(3, 4),
                EnemyRiver(4, 5),
                EnemyRiver(5, 6),
                EnemyRiver(6, 7),
                EnemyRiver(7, 0),
                MyRiver(7, 1),
                EnemyRiver(1, 3),
                EnemyRiver(3, 5),
                MyRiver(5, 7)
            };
            yield return GetTestCase("example2", new Map(Sites(8), example2Rivers, new[] {1, 5}), new Future[0], 10);
        }

        private static TestCaseData GetTestCase(string name, Map map, Future[] futures, int score)
        {
            return new TestCaseData(map, futures).SetName(name).Returns(score);
        }

        private static Site[] Sites(int size)
        {
            return Enumerable.Range(0, size).Select(e => new Site {Id = e}).ToArray();
        }

        private static River MyRiver(int source, int target)
        {
            return new River(source, target, PUNTER_ID);
        }

        private static River EnemyRiver(int source, int target)
        {
            return new River(source, target, 2);
        }

        private static Future Future(int source, int target)
        {
            return new Future(source, target);
        }
    }
}