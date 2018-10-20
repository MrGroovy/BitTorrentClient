using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Swarm
{
    public class SystemTcpClient : ITcpClient, IDisposable
    {
        private TcpClient tcpClient;

        public SystemTcpClient()
        {
            tcpClient = new TcpClient();
            tcpClient.Client.ReceiveTimeout = 5000;
            tcpClient.SendTimeout = 5000;
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
