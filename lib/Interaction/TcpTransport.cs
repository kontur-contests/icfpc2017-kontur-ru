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
    class TcpTransport : ITransport
    {
        private TcpClient client;

        public TcpTransport(int port)
        {
            client = new TcpClient();
            var ipHostInfo = Dns.Resolve("punter.inf.ed.ac.uk");
            var ipAddress = ipHostInfo.AddressList[0];

            client.Connect(ipAddress.ToString(), port);
        }

        public void Write(string data)
        {
            var message = System.Text.Encoding.ASCII.GetBytes("Testing");
            using (var stream = client.GetStream())
            {
                stream.Write(message, 0, message.Length);
                stream.Close();
            }
        }

        public string Read()
        {
            string answer;
            using (var stream = client.GetStream())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    answer = sr.ReadLine();
                }
            }
            return answer;
        }
    }
}
