using System.Net;

namespace Lib.Bittorrent.Tracker.Dto
{
    public class PeerDto
    {
        public IPAddress Ip { get; set; }
        public int Port { get; set; }
        public byte[] PeerId { get; set; }

        public PeerDto()
        {
        }

        public PeerDto(IPAddress ip, int port, byte[] peerId)
        {
            Ip = ip;
            Port = port;
            PeerId = peerId;
        }
    }
}
