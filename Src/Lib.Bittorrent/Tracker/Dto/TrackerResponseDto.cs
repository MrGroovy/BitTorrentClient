using System.Collections.Generic;
using System.Net;

namespace Lib.Bittorrent.Tracker.Dto
{
    public class TrackerResponseDto
    {
        public int Interval { get; set; }
        public List<PeerDto> Peers { get; }

        public TrackerResponseDto()
        {
            Interval = 60;
            Peers = new List<PeerDto>();
        }

        public void AddPeer(IPAddress ip, int port, byte[] peerId)
        {
            Peers.Add(new PeerDto(ip, port, peerId));
        }

        public void AddPeers(List<(IPAddress ip, int port, byte[] peerId)> peers)
        {
            foreach (var (ip, port, peerId) in peers)
                AddPeer(ip, port, peerId);
        }
    }
}
