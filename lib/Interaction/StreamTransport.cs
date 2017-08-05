using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NLog;

namespace lib.Interaction
{
    public class StreamTransport : ITransport
    {
        private readonly TextReader reader;
        private readonly TextWriter writer;
        private readonly Logger log = LogManager.GetLogger(nameof(TcpTransport));

        public static StreamTransport PipeTransport()
        {
            return new StreamTransport(Console.In, Console.Out);
        }

        public static StreamTransport TcpTransport(int port)
        {
            var ipHostInfo = Dns.Resolve("punter.inf.ed.ac.uk");
            var ipAddress = ipHostInfo.AddressList[0];
            return TcpTransport(new IPEndPoint(ipAddress, port));
        }

        public static StreamTransport TcpTransport(IPEndPoint endPoint)
        {
            var client = new TcpClient();
            client.Connect(endPoint);
            client.ReceiveTimeout = 10000;
            client.SendTimeout = 10000;

            var stream = client.GetStream();
            return new StreamTransport(new StreamReader(stream, Encoding.ASCII), new StreamWriter(stream, Encoding.ASCII));
        }

        public StreamTransport(TextReader reader, TextWriter writer)
        {
            this.reader = reader;
            this.writer = writer;
        }

        public void Write(string data)
        {
            var strToSend = data.Length + ":" + data;
            log.Info($"Write {strToSend}");
            writer.Write(strToSend);
        }

        public string Read()
        {
            var n = ReadN();
            var str = new StringBuilder();

            var myReadBuffer = new char[1024];
            if (n < 1024)
                myReadBuffer = new char[n];

            while (str.Length < n)
            {
                while (reader.Peek() == -1)
                    Thread.Sleep(5);

                var readedCharCount = reader.ReadBlock(myReadBuffer, 0, myReadBuffer.Length);
                str.Append(new string(myReadBuffer, 0, readedCharCount));
            }

            log.Info($"Read {str}");
            return str.ToString();
        }

        private int ReadN()
        {
            var str = new StringBuilder();
            while (true)
            {
                while (reader.Peek() == -1)
                    Thread.Sleep(20);
                var @char = Convert.ToChar(reader.Read());
                if (@char == ':')
                    break;
                str.Append(@char);
            }
            return int.Parse(str.ToString());
        }
    }
}