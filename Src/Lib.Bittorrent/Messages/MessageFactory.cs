using Lib.Bittorrent.Messages.Commands;
using Lib.Bittorrent.Messages.Events;
using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using Lib.Bittorrent.Tracker.Client;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Lib.Bittorrent.Messages
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

        public ReadMetaInfoFromFileCmd CreateReadMetaInfoFromFileCommand(string filePath) =>
            new ReadMetaInfoFromFileCmd(
                filePath,
                state);

        public CallTrackerCmd CreateCallTrackerCommand() =>
            new CallTrackerCmd(
                trackerClient,
                state,
                logFactory.CreateLogger<CallTrackerCmd>());

        public DecideWhatToDoCmd CreateDecideWhatToDoCommand() =>
            new DecideWhatToDoCmd(
                state);

        public ConnectToPeerCmd CreateConnectToPeerCommand(IPAddress ip, int port, byte[] peerId) =>
            new ConnectToPeerCmd(
                ip,
                port,
                peerId,
                swarm,
                state,
                logFactory.CreateLogger<ConnectToPeerCmd>());

        public HandshakeReceivedEvent CreateHandshakeReceivedEvent(IPAddress ip, int port, HandshakeMessage handshake) =>
            new HandshakeReceivedEvent(
                ip,
                port,
                handshake,
                logFactory.CreateLogger<HandshakeReceivedEvent>());

        public ChokeReceivedEvent CreateChokeReceivedEvent(IPAddress ip, int port, ChokeMessage choke) =>
            new ChokeReceivedEvent(
                ip,
                port,
                choke,
                state,
                logFactory.CreateLogger<ChokeReceivedEvent>());

        public UnchokeReceivedEvent CreateUnchokeReceivedEvent(IPAddress ip, int port, UnchokeMessage unchoke) =>
            new UnchokeReceivedEvent(
                ip,
                port,
                unchoke,
                state,
                logFactory.CreateLogger<UnchokeReceivedEvent>());

        public HaveReceivedEvent CreateHaveReceivedEvent(IPAddress ip, int port, HaveMessage have) =>
            new HaveReceivedEvent(
                ip,
                port,
                have,
                state,
                logFactory.CreateLogger<HaveReceivedEvent>());

        public BitfieldReceivedEvent CreateBitfieldReceivedEvent(IPAddress ip, int port, BitfieldMessage bitfield) =>
            new BitfieldReceivedEvent(
                ip,
                port,
                bitfield,
                state,
                logFactory.CreateLogger<BitfieldReceivedEvent>());        

        public KeepAliveReceivedEvent CreateKeepAliveReceivedEvent(IPAddress ip, int port) =>
            new KeepAliveReceivedEvent(
                ip,
                port,
                logFactory.CreateLogger<KeepAliveReceivedEvent>());

        public ReceiveErrorEvent CreateReceiveErrorEvent(IPAddress ip, int port) =>
            new ReceiveErrorEvent(
                ip,
                port,
                state,
                swarm);
    }
}
