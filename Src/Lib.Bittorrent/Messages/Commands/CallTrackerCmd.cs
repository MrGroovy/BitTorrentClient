using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Tracker.Client;
using Lib.Bittorrent.Tracker.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages.Commands
{
    public class CallTrackerCmd : Message
    {
        private ITrackerClient trackerClient;
        private TorrentState state;
        private ILogger<CallTrackerCmd> log;

        public CallTrackerCmd(ITrackerClient trackerClient, TorrentState state, ILogger<CallTrackerCmd> log)
        {
            this.trackerClient = trackerClient;
            this.state = state;
            this.log = log;
        }

        public override async Task Execute(IMessageLoop loop)
        {
            log.LogInformation("Calling tracker...");

            var request = new TrackerRequestDto();
            request.InfoHash = state.MetaInfo.InfoHash;
            request.PeerId = state.PeerId;
            request.Port = state.PublicPort;
            request.Uploaded = state.Uploaded;
            request.Downloaded = state.Downloaded;
            request.Left = state.MetaInfo.TotalLength;

            try
            {
                TrackerResponseDto response = await trackerClient.CallTracker(state.MetaInfo.Announce, request);

                foreach (PeerDto peer in response.Peers)
                {
                    state.DiscoverPeer(peer.Ip, peer.Port, peer.PeerId);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Call tracker failed.");
            }
        }
    }
}
