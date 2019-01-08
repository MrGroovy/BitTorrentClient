using System;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent
{
    public interface IPeerSocket : IDisposable
    {
        IPAddress Ip { get; }
        int Port { get; }

        void Close();
        Task Connect(IPAddress ip, int port, TimeSpan timeout);

        Task SendHandshake(string protocolString, byte[] reserved, byte[] infoHash, byte[] peerId);

        Task<Message> ReceiveMessage();        
    }
}