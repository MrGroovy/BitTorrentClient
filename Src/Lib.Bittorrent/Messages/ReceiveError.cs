using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages
{
    public class ReceiveError : Message
    {
        private IPAddress ip;
        private int port;
        private TorrentState state;
        private IPeerSwarm swarm;

        public ReceiveError(IPAddress ip, int port, TorrentState state, IPeerSwarm swarm)
        {
            this.ip = ip;
            this.port = port;
            this.state = state;
            this.swarm = swarm;
        }

        public override Task Execute(IMessageLoop loop)
        {
            throw new NotImplementedException();
        }
    }
}
