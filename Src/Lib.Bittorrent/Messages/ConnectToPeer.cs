using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages
{
    public class ConnectToPeer : Message
    {
        private IPAddress ip;
        private int port;
        private byte[] peerId;

        private IPeerSwarm swarm;
        private TorrentState state;
        private ILogger<ConnectToPeer> log;

        public ConnectToPeer(IPAddress ip, int port, byte[] peerId, IPeerSwarm swarm, TorrentState state, ILogger<ConnectToPeer> log)
        {
            this.ip = ip;
            this.port = port;
            this.peerId = peerId;

            this.swarm = swarm;
            this.state = state;
            this.log = log;
        }

        public override async Task Execute(MessageLoop loop)
        {
            try
            {
                await swarm.Connect(ip, port, peerId);
                await state.RunInLock(() => state.SetPeerState(ip, port, PeerState.Connected));
                await swarm.SendHandshake(
                    ip,
                    port,
                    new HandshakeMessage(
                        "BitTorrent protocol",
                        new byte[8],
                        state.MetaInfo.InfoHash,
                        state.PeerId));
            }
            catch (Exception ex)
            {
                await state.RunInLock(() => state.SetPeerState(ip, port, PeerState.Disconnected));

                log.LogError(ex, $"Failed to connect to peer and handshake.");
            }
        }
    }
}
