using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using FluentAssertions;
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
            var client = StreamTransport.TcpTransport(9019);

            client.Write("{\"me\":\"Ya\"}");
            client.Read().Should().BeEquivalentTo("{\"you\":\"Ya\"}");
        }
    }
}