using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Swarm
{
    public class SystemTcpClient : ITcpClient, IDisposable
    {
        private TcpClient tcpClient;

        public bool Connected => tcpClient.Connected;

        public SystemTcpClient()
        {
            tcpClient = new TcpClient();
        }

        public async Task ConnectAsync(IPAddress address, int port)
        {
            await tcpClient.ConnectAsync(address, port);
        }

        public void Close()
        {
            tcpClient.Close();
        }

        public async Task WriteAsync(byte[] buffer)
        {
            await tcpClient
                .GetStream()
                .WriteAsync(buffer, 0, buffer.Length);
        }

        public async Task<int> ReadAsync(byte[] buffer)
        {
            return await tcpClient
                .GetStream()
                .ReadAsync(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            tcpClient.Dispose();
        }
    }
}
