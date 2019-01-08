using System.Net;

namespace Lib.Bittorrent
{
    public class Peer
    {
        public IPAddress Ip { get; set; }
        public int Port { get; set; }
        public byte[] PeerId { get; set; }

        public Peer(IPAddress ip, int port)
        {
            Ip = ip;
            Port = port;
        }
    }
}
