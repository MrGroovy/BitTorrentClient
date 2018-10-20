using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Swarm
{
    public interface IPeerSwarm
    {
        Task Connect(IPAddress ip, int port, byte[] clientId);
        Task SendHandshake(IPAddress ip, int port, HandshakeMessage handshakeMsg);
    }
}