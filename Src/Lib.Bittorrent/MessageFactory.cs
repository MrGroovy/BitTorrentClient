using Lib.Bittorrent.Messages;
using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using Lib.Bittorrent.Tracker.Client;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Lib.Bittorrent
{
    public class MessageFactory
    {
        private TorrentState state;
        private TrackerClient trackerClient;
        private IPeerSwarm swarm;
        private ILoggerFactory logFactory;

        public void SetDependencies(TorrentState state, TrackerClient trackerClient, IPeerSwarm swarm, ILoggerFactory logFactory)
        {
            this.state = state;
            this.trackerClient = trackerClient;
            this.swarm = swarm;
            this.logFactory = logFactory;
        }

        public ReadMetaInfoFromFile CreateReadMetaInfoFromFileMessage(string filePath) =>
            new ReadMetaInfoFromFile(
                filePath,
                state);

        public CallTracker CreateCallTrackerMessage() =>
            new CallTracker(
                trackerClient,
                state,
                logFactory.CreateLogger<CallTracker>());  

        public ConnectToPeer CreateConnectToPeerMessage(IPAddress ip, int port, byte[] peerId) =>
            new ConnectToPeer(
                ip,
                port,
                peerId,
                swarm,
                state,
                logFactory.CreateLogger<ConnectToPeer>());

        public HandshakeReceived CreateHandshakeReceivedMessage(IPAddress ip, int port, HandshakeMessage handshake) =>
            new HandshakeReceived(
                ip,
                port,
                handshake,
                logFactory.CreateLogger<HandshakeReceived>());

        public KeepAliveReceived CreateKeepAliveReceivedMessage(IPAddress ip, int port) =>
            new KeepAliveReceived(
                ip,
                port);

        public ReceiveError CreateReceiveErrorMessage(IPAddress ip, int port) =>
            new ReceiveError(
                ip,
                port,
                state,
                swarm);

        public DecideWhatToDo CreateDecideWhatToDoMessage() =>
            new DecideWhatToDo(
                state);
    }
}
