using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Swarm
{
    public class SystemSocket : ISocket, IDisposable
    {
        private Socket socket;

        public bool Connected => socket.Connected;

        public SystemSocket()
        {
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task ConnectAsync(IPAddress address, int port)
        {
            await socket.ConnectAsync(address, port);
        }

        public void Close()
        {
            socket.Close();
        }

        public async Task SendAsync(byte[] buffer)
        {
            await socket.SendAsync(buffer, 0);
        }

        public async Task<int> ReceiveAsync(byte[] buffer)
        {
            return await socket.ReceiveAsync(new Memory<byte>(buffer), SocketFlags.None);
        }

        public void Dispose()
        {
            socket.Dispose();
        }
    }
}
