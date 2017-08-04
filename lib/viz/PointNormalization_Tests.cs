using System.Collections.Generic;
using System.Drawing;
using FluentAssertions;
using NUnit.Framework;

namespace lib.viz
{
    [TestFixture]
    public class PointNormalization_Tests
    {
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(new[] { P(0, 0), P(1, 1) }, S(1, 1), S(0, 0));
                yield return new TestCaseData(new[] { P(0, 0), P(10, 10) }, S(1, 1), S(0, 0));
                yield return new TestCaseData(new[] { P(0, 0), P(10, 10) }, S(3, 3), S(1, 1));
                yield return new TestCaseData(new[] { P(-10, 0), P(0, -10), P(10, 0), P(0, 10) }, S(5, 5), S(2, 1));
            }
        }

        private static PointF P(float x, float y)
        {
            return new PointF(x, y);
        }

        private static SizeF S(float w, float h)
        {
            return new SizeF(w, h);
        }

        [TestCaseSource(nameof(TestCases))]
        public void BoundingBoxNormalization(PointF[] points, SizeF targetSize, SizeF padding)
        {
            points
                .NormalizeCoordinates(targetSize, padding)
                .GetBoundingBox()
                .ShouldBeEquivalentTo(
                    new RectangleF(
                        padding.Width,
                        padding.Height,
                        targetSize.Width - 2 * padding.Width,
                        targetSize.Height - 2 * padding.Height));
        }
    }
}