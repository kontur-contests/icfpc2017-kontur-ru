using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace lib.Interaction
{
    public class TcpTransport : ITransport
    {
        private readonly TcpClient client;
        private NetworkStream networkStream;

        public TcpTransport(int port)
        {
            client = new TcpClient();
            var ipHostInfo = Dns.Resolve("punter.inf.ed.ac.uk");
            var ipAddress = ipHostInfo.AddressList[0];

            client.Connect(ipAddress.ToString(), port);
            networkStream = client.GetStream();
        }

        public void Write(string data)
        {
            var strToSend = data.Length + ":" + data;
            Console.WriteLine(client.Connected);
            var buffer = Encoding.ASCII.GetBytes(strToSend);
                networkStream.Write(buffer, 0, buffer.Length);
            Console.WriteLine(client.Connected);
        }

        public string Read()
        {
            var  answer = new byte[1024];
            Console.WriteLine(client.Connected);
            networkStream.Read(answer, 0, 1024);

            return Encoding.ASCII.GetString(answer).Split(':')[1];
        }
    }
}
