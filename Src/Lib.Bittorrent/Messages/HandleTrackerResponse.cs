﻿using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Tracker.Dto;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages
{
    public class HandleTrackerResponse : Message
    {
        private TrackerResponseDto trackerResponse;
        private TorrentState state;
        private ILogger<HandleTrackerResponse> log;

        public HandleTrackerResponse(TrackerResponseDto trackerResponse, TorrentState state, ILogger<HandleTrackerResponse> log)
        {
            this.trackerResponse = trackerResponse;
            this.state = state;
            this.log = log;
        }

        public override Task Execute(MessageLoop loop)
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
