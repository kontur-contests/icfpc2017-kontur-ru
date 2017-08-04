using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace worker
{
    [TestFixture]
    public class StrategyTests
    {
        [Test]
        public void DummySumPlayerStrategy_Should()
        {
            var strategy = new DummySumPlayerStrategy();
            var playersIn = new List<PlayerWithParams>
            {
                new PlayerWithParams
                {
                    Name = "zero",
                    Params = new Dictionary<string, double> {{"param1", 0}, {"param2", 0}, {"param3", 0}}
                },
                new PlayerWithParams
                {
                    Name = "two",
                    Params = new Dictionary<string, double> {{"param1", 1}, {"param2", 0}, {"param3", 1}}
                },
                new PlayerWithParams
                {
                    Name = "one",
                    Params = new Dictionary<string, double> {{"param1", 0}, {"param2", 0}, {"param3", 1}}
                },
            };

            var playersOut = strategy.Play(playersIn);
            var bestPlayerOut = playersOut.First().Item1;
            
            Assert.AreEqual(2, bestPlayerOut.Rank);
            Assert.AreEqual("two", bestPlayerOut.Name);
        }
    }
}