using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Tracker.Dto;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages
{
    public class TrackerResponseReceived : Message
    {
        private TrackerResponseDto trackerResponse;
        private TorrentState state;
        private ILogger<TrackerResponseReceived> log;

        public TrackerResponseReceived(TrackerResponseDto trackerResponse, TorrentState state, ILogger<TrackerResponseReceived> log)
        {
            this.trackerResponse = trackerResponse;
            this.state = state;
            this.log = log;
        }

        public override Task Execute(IMessageLoop loop)
        {
            log.LogInformation("Handling tracker response.");

            foreach (PeerDto peer in trackerResponse.Peers)
            {
                state.DiscoverPeer(peer.Ip, peer.Port, peer.PeerId);
            }

            return Task.CompletedTask;
        }
    }
}
