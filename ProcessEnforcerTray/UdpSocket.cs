using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ProcessEnforcerTray
{
    public class UDPSocket
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public event Action<string, EndPoint> MessageReceived;

        public void Server(IPAddress address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(address, port));
            Receive();
        }

        public void Client(IPAddress address, int port)
        {
            _socket.Connect(address, port);
            Receive();
        }

        public void Send(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);
                Logging.Log($"SEND: {bytes}, {text}");
            }, state);
        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);

                // Extract the received message
                string message = Encoding.ASCII.GetString(so.buffer, 0, bytes);

                // Log the received message
                Logging.Log($"RECV: {epFrom.ToString()}: {bytes}, {message}");

                // Trigger the MessageReceived event
                MessageReceived?.Invoke(message, epFrom);

                // Continue listening for the next message
                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
            }, state);
        }
    }
}
