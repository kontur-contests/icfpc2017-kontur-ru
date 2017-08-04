using System;
using FakeItEasy;
using lib.Interaction.Internal;
using Newtonsoft.Json;
using NUnit.Framework;

namespace lib.Interaction
{
    [TestFixture]
    public class OnlineProtocol_Tests
    {
        [Test]
        public void TestHandShake()
        {
            var transport = A.Fake<ITransport>();
            var gameTransport = new OnlineProtocol(transport);
            A.CallTo(() => transport.Read()).Returns("{\"you\":\"player\"}");

            gameTransport.HandShake("player");

            var expectedToWrite = "{\"me\":\"player\"}";
            A.CallTo(() => transport.Write(expectedToWrite)).MustHaveHappened();
        }

        [Test]
        public void TestHandShakeWithoutMock()
        {
            var transport = new TcpTransport(9011);
            var gameTransport = new OnlineProtocol(transport);

            gameTransport.HandShake("playёr");
            var setup = gameTransport.ReadSetup();
            Console.WriteLine(setup.Id);
        }

        [Test]
        public void TestReadSetup()
        {
            var transport = A.Fake<ITransport>();
            var gameTransport = new OnlineProtocol(transport);
            var setup = new Setup
            {
                Id = "id"
            };
            var data = JsonConvert.SerializeObject(setup);

            A.CallTo(() => transport.Read()).Returns(data);

            gameTransport.ReadSetup();

            var expectedToWrite = "{\"ready\":\"id\"}";
            A.CallTo(() => transport.Write(expectedToWrite)).MustHaveHappened();
        }
    }
}