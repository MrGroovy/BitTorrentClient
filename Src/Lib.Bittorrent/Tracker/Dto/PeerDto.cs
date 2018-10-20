using System.Net;

namespace Lib.Bittorrent.Tracker.Dto
{
    public class PeerDto
    {
        public IPAddress Ip { get; }
        public int Port { get; }
        public byte[] PeerId { get; }

        public PeerDto(IPAddress ip, int port, byte[] peerId)
        {
            Ip = ip;
            Port = port;
            PeerId = peerId;
        }
    }
}
