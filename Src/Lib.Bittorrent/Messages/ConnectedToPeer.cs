using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages
{
    public class ConnectedToPeer : Message
    {
        private IPAddress ip;
        private int port;
        private TorrentState state;
        private IPeerSwarm swarm;
        private ILogger<ConnectedToPeer> log;

        public ConnectedToPeer(IPAddress ip, int port, TorrentState state, IPeerSwarm swarm, ILogger<ConnectedToPeer> log)
        {
            this.ip = ip;
            this.port = port;
            this.state = state;
            this.swarm = swarm;
            this.log = log;
        }

        public override async Task Execute(IMessageLoop loop)
        {
            log.LogInformation("Connected to peer.");

            state.SetPeerState(ip, port, PeerState.Connected);
            
            //await swarm.SendHandshake(
            //    ip,
            //    port,
            //    new HandshakeMessage(
            //        "BitTorrent protocol",
            //        new byte[8],
            //        state.MetaInfo.InfoHash,
            //        state.PeerId));
        }
    }
}
