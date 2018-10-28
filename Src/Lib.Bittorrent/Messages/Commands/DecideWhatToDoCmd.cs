using Lib.Bittorrent.StateManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages.Commands
{
    public class DecideWhatToDoCmd : Message
    {
        private TorrentState state;
        private Random random;

        private const int targetSwarmSize = 1;

        public DecideWhatToDoCmd(TorrentState state)
        {
            this.state = state;
            this.random = new Random();
        }

        public override async Task Execute(IMessageLoop loop)
        {
            await state.RunInLock(() => ConnectToMorePeers(loop));
        }

        private void ConnectToMorePeers(IMessageLoop loop)
        {
            if (state.Peers.Count > 0 && state.GetPeersInSwarm().Count < targetSwarmSize)
            {
                List<Peer> notInSwarm = state.GetPeersOutsideSwarm();
                Peer randomPeer = notInSwarm[random.Next(0, notInSwarm.Count)];

                state.SetPeerState(randomPeer.Ip, randomPeer.Port, PeerState.Connecting);
                loop.PostConnectToPeerCommand(
                    randomPeer.Ip,
                    randomPeer.Port,
                    randomPeer.PeerId);
            }
        }
    }
}
