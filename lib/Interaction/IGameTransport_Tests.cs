using System;
using FakeItEasy;
using NUnit.Framework;

namespace lib.Interaction
{
    [TestFixture]
    public class IGameTransport_Tests
    {
        [Test]
        public void TestHandShake()
        {
            var x = "a";
            Console.WriteLine($"{{\"me\":\"{x}\"}}");
            var transport = A.Fake<OfflineHighTransport>();
            var gameTransport = new OnlineInteraction(transport);
            A.CallTo(() => transport.Read()).Returns("{\"you\":\"player\"}");

            gameTransport.HandShake("player");

            var expectedToWrite = "{\"me\":\"player\"}";
            A.CallTo(() => transport.Write(expectedToWrite)).MustHaveHappened();
        }
    }
}
