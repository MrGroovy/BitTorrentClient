using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Tracker.Client;
using Lib.Bittorrent.Tracker.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages
{
    public class CallTracker : Message
    {
        private TrackerClient trackerClient;
        private TorrentState state;
        private MessageFactory msgFactory;
        private ILogger<CallTracker> log;

        public CallTracker(TrackerClient trackerClient, TorrentState state, MessageFactory msgFactory, ILogger<CallTracker> log)
        {
            this.trackerClient = trackerClient;
            this.state = state;
            this.msgFactory = msgFactory;
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
                loop.PostTrackerResponseReceivedMessage(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Call tracker failed.");
            }
        }
    }
}
