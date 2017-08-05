using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace lib.Interaction
{
    public class TcpTransport : ITransport
    {
        private readonly TcpClient client;
        private readonly NetworkStream networkStream;
        private readonly Logger log = LogManager.GetLogger(nameof(TcpTransport));

        public TcpTransport(int port)
        {
            client = new TcpClient();
            var ipHostInfo = Dns.Resolve("punter.inf.ed.ac.uk");
            var ipAddress = ipHostInfo.AddressList[0];

            client.Connect(ipAddress.ToString(), port);
            client.ReceiveTimeout = 10000;
            client.SendTimeout = 10000;
            networkStream = client.GetStream();
        }

        public void Write(string data)
        {
            var strToSend = data.Length + ":" + data;
            log.Info($"Write {strToSend}");
            var buffer = Encoding.ASCII.GetBytes(strToSend);
            networkStream.Write(buffer, 0, buffer.Length);
        }


        public string Read()
        {
            var n = ReadN();
            var sb = new StringBuilder();

            var myReadBuffer = new byte[1024];
            if (n < 1024)
                myReadBuffer = new byte[n];

            while (sb.Length < n)
            {
                while (!networkStream.DataAvailable)
                    Thread.Sleep(5);

                var numberOfBytesRead = networkStream.Read(myReadBuffer, 0, myReadBuffer.Length);
                sb.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
            }

            sb.Remove(sb.Length - 1, 1);
            log.Info($"Read {sb}");
            return sb.ToString();
        }

        private int ReadN()
        {
            var nStr = new StringBuilder();
            var buffer = new byte[1];
            while (Convert.ToChar(buffer[0]) != ':')
            {
                while (!networkStream.DataAvailable)
                    Thread.Sleep(20);
                networkStream.Read(buffer, 0, 1);
                nStr.AppendFormat("{0}", Encoding.ASCII.GetString(buffer, 0, 1));
            }
            nStr.Remove(nStr.Length - 1, 1);
            return int.Parse(nStr.ToString());
        }
    }
}