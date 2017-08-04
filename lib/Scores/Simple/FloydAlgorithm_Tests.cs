using System.Collections.Generic;
using NUnit.Framework;

namespace lib.Scores.Simple
{
    [TestFixture]
    public class FloydAlgorithm_Tests
    {
        [TestCaseSource(nameof(GetTestData))]
        public long[][] Run(Map map)
        {
            return FloydAlgorithm.Run(map);
        }

        private static IEnumerable<TestCaseData> GetTestData()
        {
            var sites = new[]
            {
                new Site {Id = 0},
                new Site {Id = 1},
                new Site {Id = 2}
            };

            var rivers = new[]
            {
                new River(0, 1),
                new River(1, 2),
                new River(2, 0)
            };

            yield return new TestCaseData(new Map { Sites = sites, Rivers = rivers })
                .Returns(new[] {new[] {0, 1, 1}, new[] {1, 0, 1}, new[] {1, 1, 0}});
        }
    }
}