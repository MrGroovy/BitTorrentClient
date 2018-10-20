using Lib.Bittorrent.StateManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages
{
    public class DecideWhatToDo : Message
    {
        private TorrentState state;
        private MessageFactory msgFactory;
        private Random random;

        private const int targetSwarmSize = 1;

        public DecideWhatToDo(TorrentState state, MessageFactory msgFactory)
        {
            this.state = state;
            this.msgFactory = msgFactory;
            this.random = new Random();
        }

        public override async Task Execute(MessageLoop loop)
        {
            await state.RunInLock(() => ConnectToMorePeers(loop));
        }

        private void ConnectToMorePeers(MessageLoop loop)
        {
            if (state.Peers.Count > 0 && state.GetPeersInSwarm().Count < targetSwarmSize)
            {
                List<Peer> notInSwarm = state.GetPeersOutsideSwarm();
                Peer randomPeer = notInSwarm[random.Next(0, notInSwarm.Count)];

                state.SetPeerState(randomPeer.Ip, randomPeer.Port, PeerState.Connecting);
                loop.Post(
                    msgFactory.CreateConnectToPeerMessage(
                        randomPeer.Ip,
                        randomPeer.Port,
                        randomPeer.PeerId));
            }
        }
    }
}
