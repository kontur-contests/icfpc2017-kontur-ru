using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace lib.Interaction
{
    // Asynchronous Client Socket Example
    // http://msdn.microsoft.com/en-us/library/bew39x2a.aspx
    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.
        public Socket workSocket;

        // Size of receive buffer.
        public const int BufferSize = 256;

        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];

        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class Client
    {
        public Client()
        {
            var ipHostInfo = Dns.Resolve("punter.inf.ed.ac.uk");
            var ipAddress = ipHostInfo.AddressList[0];
            var remoteEp = new IPEndPoint(ipAddress, Port);

            // Create a TCP/IP socket.
            socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote endpoint.
            socket.BeginConnect(
                remoteEp,
                ConnectCallback, socket);
            connectDone.WaitOne();
            Console.WriteLine("Connected!");
        }

        // The port number for the remote device.
        private const int Port = 9026;

        // ManualResetEvent instances signal completion.
        private readonly ManualResetEvent connectDone =
            new ManualResetEvent(false);

        private readonly ManualResetEvent sendDone =
            new ManualResetEvent(false);

        private readonly ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        // The response from the remote device.
        private string response = string.Empty;

        private readonly Socket socket;

        public void StartClient()
        {
            // Connect to a remote device.
            try
            {
                SendMessage(socket, "{\"me\":\"player\"}");

                // Receive the response from the remote device.
                var message = ReceiveMessage(socket);
                //Console.WriteLine(message);

                // Release the socket.
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private string ReceiveMessage(Socket client)
        {
            Receive(client);
            receiveDone.WaitOne();

            return response;
        }

        private void SendMessage(Socket client, string message)
        {
            var messageWithLength = message.Length + ":" + message;
            Send(client, messageWithLength);
            sendDone.WaitOne();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                var client = (Socket) ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine(
                    "Socket connected to {0}",
                    client.RemoteEndPoint);

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                var state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(
                    state.buffer, 0, StateObject.BufferSize, 0,
                    ReceiveCallback, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                var state = (StateObject) ar.AsyncState;
                var client = state.workSocket;

                // Read data from the remote device.
                var bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.
                    client.BeginReceive(
                        state.buffer, 0, StateObject.BufferSize, 0,
                        ReceiveCallback, state);
                }
                else
                {
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1)
                        response = state.sb.ToString();
                    // Signal that all bytes have been received.
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Send(Socket client, string data)
        {
            // Convert the string data to byte data using ASCII encoding.
            var byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(
                byteData, 0, byteData.Length, 0,
                SendCallback, client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                var client = (Socket) ar.AsyncState;

                // Complete sending the data to the remote device.
                var bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}