using System;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Swarm
{
    public interface ITcpClient : IDisposable
    {
        Task ConnectAsync(IPAddress address, int port);
        bool Connected { get; }
        void Close();
        Task WriteAsync(byte[] buffer);
        Task<int> ReadAsync(byte[] buffer);
    }
}
