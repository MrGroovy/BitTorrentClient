using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages.Events
{
    public class ReceiveErrorEvent : Message
    {
        private IPAddress ip;
        private int port;

        private TorrentState state;
        private IPeerSwarm swarm;

        public ReceiveErrorEvent(IPAddress ip, int port, TorrentState state, IPeerSwarm swarm)
        {
            this.ip = ip;
            this.port = port;
            this.state = state;
            this.swarm = swarm;
        }

        public override async Task Execute(IMessageLoop loop)
        {
            await state.RunInLock(() => state.SetPeerState(ip, port, PeerState.Disconnected));
            swarm.Remove(ip, port);
        }
    }
}
