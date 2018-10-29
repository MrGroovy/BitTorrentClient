using System;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Swarm
{
    public interface IPeerClient : IDisposable
    {
        byte[] ClientId { get; }
        IPAddress Ip { get; }
        int Port { get; }

        void Close();
        Task Connect(IPAddress ip, int port, TimeSpan timeout);

        Task SendHandshake(HandshakeMessage handshake);

        Task<HandshakeMessage> ReceiveHandshakeMessage();
        Task<ProtocolMessage> ReceiveMessage();        
    }
}