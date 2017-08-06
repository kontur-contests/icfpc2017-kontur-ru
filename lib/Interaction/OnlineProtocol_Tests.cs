using System;
using FakeItEasy;
using lib.Interaction.Internal;
using lib.Structures;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

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
            A.CallTo(() => transport.Read<HandshakeIn>(A<int>.Ignored)).Returns(new HandshakeIn{you = "player"});

            gameTransport.HandShake("player");

            A.CallTo(() => transport.Write(A<HandshakeOut>.That.Matches(o => o.me == "player"))).MustHaveHappened();
        }

        [Test]
        [Explicit]
        public void TestHandShakeWithoutMock()
        {
            var transport = new TcpTransport(9011);
            var gameTransport = new OnlineProtocol(transport);

            gameTransport.HandShake("player");
            var setup = gameTransport.ReadSetup();
            Console.WriteLine(setup.punter);
        }

        [Test]
        public void TestReadSetup()
        {
            var transport = A.Fake<ITransport>();
            var gameTransport = new OnlineProtocol(transport);
            var setup = new In
            {
                punter = 1
            };
            A.CallTo(() => transport.Read<In>(A<int?>.Ignored)).Returns(setup);
            gameTransport.ReadSetup();
            A.CallTo(() => transport.Read<In>(A<int?>.Ignored)).MustHaveHappened();
        }
    }
}