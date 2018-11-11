using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lib.Bittorrent.StateManagement
{
    public class TorrentState : ITorrentState
    {
        public byte[] PeerId => Encoding.UTF8.GetBytes("-RV0001-483513395306");
        public int PublicPort => 6882;
        public int Uploaded => 0;
        public int Downloaded => 0;

        public MetaInfo MetaInfo { get; set; }
        public int NumberOfPieces { get { return MetaInfo.NumPieces; } }

        public List<Peer> Peers { get; private set; }

        private ILogger<TorrentState> log;
        private SemaphoreSlim locker;

        public TorrentState(ILogger<TorrentState> log)
        {
            MetaInfo = null;
            Peers = new List<Peer>();
            this.log = log;
            this.locker = new SemaphoreSlim(1, 1);
        }

        public void DiscoverPeer(IPAddress ip, int port, byte[] peerId)
        {
            Pre(ip is IPAddress, nameof(ip));
            Pre(port >= 0, nameof(port));
            Pre(peerId.Length == 20, nameof(peerId));

            if (!(GetPeer(ip, port) is Peer))
            {
                log.LogInformation("Peer discovered: {ip}:{port}", ip, port);
                Peers.Add(new Peer(
                    ip,
                    port,
                    peerId,
                    MetaInfo.NumPieces));
            }
        }

        public void SetPeerState(IPAddress ip, int port, PeerState newState)
        {
            if (GetPeer(ip, port) is Peer peer)
            {
                peer.State = newState;
            }
        }

        public List<Peer> GetPeersOutsideSwarm()
        {
            return Peers
                .Where(p => !p.IsInSwarm)
                .ToList();
        }

        public List<Peer> GetPeersInSwarm()
        {
            return Peers
                .Where(p => p.IsInSwarm)
                .ToList();
        }

        private Peer GetPeer(IPAddress ip, int port)
        {
            return Peers.FirstOrDefault(p => p.Ip == ip && p.Port == port);
        }

        public void MarkPieceAsAvailable(IPAddress ip, int port, int pieceIndex)
        {
            if (GetPeer(ip, port) is Peer peer)
            {
                peer.MarkPieceAsAvailable(pieceIndex);
            }
        }

        public void MarkPiecesAsAvailable(IPAddress ip, int port, bool[] pieceIndexes)
        {
            for (int i = 0; i < pieceIndexes.Length; i++)
            {
                if (pieceIndexes[i])
                {
                    MarkPieceAsAvailable(ip, port, i);
                }
            }
        }

        public void SetIsHeChoking(IPAddress ip, int port, bool isHeChoking)
        {
            if (GetPeer(ip, port) is Peer peer)
            {
                peer.IsHeChoking = isHeChoking;
            }
        }

        public async Task RunInLock(Func<Task> action)
        {
            await locker.WaitAsync();

            try
            {
                await action.Invoke();
            }
            finally
            {
                locker.Release();
            }
        }

        public async Task RunInLock(Action action)
        {
            await RunInLock(() =>
            {
                action.Invoke();
                return Task.CompletedTask;
            });
        }

        private void Pre(bool condition, string name)
        {
            if (!condition)
                throw new ArgumentException($"Preconditions of argument '{name}' violated.");
        }
    }
}
