using System;
using FakeItEasy;
using lib.Interaction.Internal;
using NUnit.Framework;

namespace lib.Interaction
{
    [TestFixture]
    public class OnlineHighTransport_Tests
    {
        [Test]
        public void TestHandShake()
        {
            var transport = A.Fake<ITransport>();
            var gameTransport = new OnlineHighTransport(transport);
            A.CallTo(() => transport.Read()).Returns("{\"you\":\"player\"}");

            gameTransport.HandShake("player");

            var expectedToWrite = "{\"me\":\"player\"}";
            A.CallTo(() => transport.Write(expectedToWrite)).MustHaveHappened();
        }

        [Test]
        public void TestReadSetup()
        {
            var transport = A.Fake<ITransport>();
            var gameTransport = new OnlineHighTransport(transport);
            A.CallTo(() => transport.Read()).Returns("{\"you\":\"player\"}");

            gameTransport.HandShake("player");

            var expectedToWrite = "{\"me\":\"player\"}";
            A.CallTo(() => transport.Write(expectedToWrite)).MustHaveHappened();
        }
    }
}
