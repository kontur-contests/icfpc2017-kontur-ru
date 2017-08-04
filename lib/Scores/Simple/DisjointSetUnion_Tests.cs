using NUnit.Framework;

namespace lib.Scores.Simple
{
    [TestFixture]
    public class DisjointSetUnion_Tests
    {
        [Test]
        public void Test()
        {
            var disjointSetUnion = new DisjointSetUnion(3);

            disjointSetUnion.Add(0, 1);
            disjointSetUnion.Add(1, 2);
            disjointSetUnion.Add(2, 0);

            Assert.IsTrue(disjointSetUnion.SameSet(0, 1));
            Assert.IsTrue(disjointSetUnion.SameSet(1, 0));
            Assert.IsTrue(disjointSetUnion.SameSet(1, 2));
            Assert.IsTrue(disjointSetUnion.SameSet(2, 1));
            Assert.IsTrue(disjointSetUnion.SameSet(0, 2));
            Assert.IsTrue(disjointSetUnion.SameSet(2, 0));
        }
    }
}