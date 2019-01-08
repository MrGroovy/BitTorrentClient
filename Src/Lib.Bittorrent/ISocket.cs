using System;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent
{
    public interface ISocket : IDisposable
    {
        Task ConnectAsync(IPAddress address, int port);
        bool Connected { get; }
        void Close();
        Task SendAsync(byte[] buffer);
        Task<int> ReceiveAsync(byte[] buffer);
    }
}
