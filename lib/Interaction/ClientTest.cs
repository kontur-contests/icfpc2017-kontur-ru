using NUnit.Framework;

namespace lib.Interaction
{
    [TestFixture]
    public class ClientTest
    {
        [Test]
        public void Test()
        {
            var client = new TcpTransport(9019);
            client.Read();
        }
    }
}