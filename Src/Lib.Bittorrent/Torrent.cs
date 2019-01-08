using Lib.Bittorrent.Tracker.Client;
using Lib.Bittorrent.Tracker.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Bittorrent
{
    public class Torrent
    {
        public MetaInfo MetaInfo => metaInfo;

        private MetaInfo metaInfo;
        private List<Peer> peers;
        private int maxConnectedPeers;
        private List<PeerClient> connectedPeers;
        //PiecesDownloaded<PieceIndex, boolean>
        //PiecesDownloading<PieceIndex, PeerClientId>
        private Random random;
        private TrackerClient trackerClient;
        private ILoggerFactory logFactory;
        private ILogger<Torrent> log;

        public Torrent(MetaInfo metaInfo, ILoggerFactory logFactory, ILogger<Torrent> log)
        {
            if (metaInfo == null) throw new ArgumentNullException(nameof(metaInfo));

            this.metaInfo = metaInfo;
            this.logFactory = logFactory;
            this.log = log;
            this.peers = new List<Peer>();
            this.maxConnectedPeers = 1;
            this.connectedPeers = new List<PeerClient>();
            this.random = new Random();
            this.trackerClient = new TrackerClient();
            _ = Run();
        }

        private async Task Run()
        {
            while (true)
            {
                try
                {
                    PrunePeerClients();
                    await CallTrackerIfNeeded();
                    ConnectToPeersIfNeeded();
                }
                catch (Exception ex)
                {
                    log.LogError($"Error in {nameof(Torrent)} {nameof(Run)} loop.", ex);
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private void PrunePeerClients()
        {
            List<PeerClient> notRunning = connectedPeers
                .Where(p => p.IsRunning is false)
                .ToList();

            if (notRunning.Count > 0)
            {
                foreach (PeerClient peerClient in notRunning)
                {
                    connectedPeers.Remove(peerClient);
                    peerClient.Dispose();
                }

                log.LogInformation("Pruned {NumClients} peer clients", notRunning.Count);
            }
        }

        private async Task CallTrackerIfNeeded()
        {
            if (peers.Count > 0)
                return;

            var request = new TrackerRequestDto();
            request.InfoHash = metaInfo.InfoHash;
            request.PeerId = Encoding.UTF8.GetBytes("-RV0001-483513395306");
            request.Port = 0;
            request.Uploaded = 0;
            request.Downloaded = 0;
            request.Left = metaInfo.TotalLength;
            TrackerResponseDto response = await trackerClient.CallTracker(metaInfo.Announce, request);

            List<Peer> newPeers = response
                .Peers
                .Where(p => !peers.Any(x => p.Ip == x.Ip && p.Port == x.Port))
                .Select(p => new Peer(p.Ip, p.Port))
                .ToList();
            peers.AddRange(newPeers);

            log.LogInformation("Discovered {NumPeers} peers", newPeers.Count);
        }

        private void ConnectToPeersIfNeeded()
        {
            if (!(connectedPeers.Count < maxConnectedPeers))
                return;

            List<Peer> notConnected = peers
                .Where(p => !connectedPeers.Any(x => p.Ip == x.Ip && p.Port == x.Port))
                .OrderBy(p => Guid.NewGuid())
                .Take(maxConnectedPeers - connectedPeers.Count)
                .ToList();

            foreach (Peer peer in notConnected)
            {
                PeerClient peerClient = new PeerClient(
                    peer.Ip,
                    peer.Port,
                    this,
                    new PeerSocket(
                        new SystemSocket(),
                        logFactory.CreateLogger<PeerSocket>()),
                    logFactory.CreateLogger<PeerClient>());
                connectedPeers.Add(peerClient);
            }
        }
    }
}
