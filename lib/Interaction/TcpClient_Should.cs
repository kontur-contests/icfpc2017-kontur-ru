using FluentAssertions;
using lib.Structures;
using NUnit.Framework;

namespace lib.Interaction
{
    [TestFixture]
    public class TcpClient_Should
    {
        [Test]
        [Explicit]
        public void DoHandshake()
        {
            var client = new TcpTransport(9019);

            client.Write(new HandshakeOut { me = "Ya" });
            client.Read<HandshakeIn>().Should().ShouldBeEquivalentTo(new HandshakeIn { you = "Ya" });
        }
    }
}