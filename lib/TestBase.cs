using FluentAssertions;
using NUnit.Framework;

namespace lib
{
    [TestFixture]
    public class TestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AssertionOptions.AssertEquivalencyUsing(
                options =>
                    options.Using<River>(ctx => ctx.Subject.Should().Be(ctx.Expectation)).WhenTypeIs<River>());
        }
    }
}