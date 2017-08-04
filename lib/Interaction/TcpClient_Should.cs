using FluentAssertions;
using NUnit.Framework;

namespace lib.Interaction
{
    [TestFixture]
    public class TcpClient_Should
    {
        [Test]
        [Explicit]
        public void MakeHandshakeAndSetup()
        {
            var client = new TcpTransport(9019);

            client.Write("{\"me\":\"Ya\"}");
            client.Read().Should().BeEquivalentTo("{\"you\":\"Ya\"}");
            client.Read().StartsWith("{\"punter\":").Should().BeTrue();
        }
    }
}