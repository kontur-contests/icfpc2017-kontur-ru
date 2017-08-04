using System.Collections.Generic;
using System.Drawing;
using FluentAssertions;
using NUnit.Framework;

namespace lib.viz
{
    [TestFixture]
    public class BoundingBox_Tests
    {
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(new PointF[0], new RectangleF(0, 0, 1, 1));
                yield return new TestCaseData(new[] { P(0, 0) }, new RectangleF(0, 0, 1, 1));
                yield return new TestCaseData(new[] { P(-1, -1) }, new RectangleF(-1, -1, 1, 1));
                yield return new TestCaseData(new[] { P(-10, -10), P(10, 10) }, new RectangleF(-10, -10, 20, 20));
                yield return new TestCaseData(
                    new[] { P(-10, 0), P(0, -10), P(10, 0), P(0, 10) }, new RectangleF(-10, -10, 20, 20));
            }
        }

        private static PointF P(float x, float y)
        {
            return new PointF(x, y);
        }

        [TestCaseSource(nameof(TestCases))]
        public void BoundingBox(PointF[] points, RectangleF bbox)
        {
            points.GetBoundingBox().ShouldBeEquivalentTo(bbox);
        }
    }
}