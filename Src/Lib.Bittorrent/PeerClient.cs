using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Bittorrent
{
    public class PeerClient : IDisposable
    {
        public IPAddress Ip { get; private set; }
        public int Port { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsHeChoking { get; private set; }
        public bool IsHeInterested { get; private set; }
        //AreWeChoking
        //AreWeInterested
        public bool[] PiecesAvailable { get; private set; }
        private Torrent torrent;
        private PeerSocket socket;
        private ILogger<PeerClient> log;

        public PeerClient(IPAddress ip, int port, Torrent torrent, PeerSocket socket, ILogger<PeerClient> log)
        {
            this.Ip = ip;
            this.Port = port;
            this.torrent = torrent;
            this.socket = socket;
            this.log = log;
            this.PiecesAvailable = new bool[torrent.MetaInfo.NumPieces];
            this.IsRunning = true;
            _ = Run();
        }

        private async Task Run()
        {
            try
            {
                await ConnectAndSendHandshake();
                await ReceiveMessages();
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error in {nameof(PeerClient)} {nameof(Run)} loop.");
            }
            finally
            {
                IsRunning = false;
            }
        }

        private async Task ConnectAndSendHandshake()
        {
            await socket.Connect(Ip, Port, TimeSpan.FromSeconds(4));
            await socket.SendHandshake(
                "BitTorrent protocol",
                new byte[8],
                torrent.MetaInfo.InfoHash,
                Encoding.UTF8.GetBytes("-RV0001-483513395306"));
        }

        private async Task ReceiveMessages()
        {
            while (true)
            {
                Message message = await socket.ReceiveMessage();
                switch (message)
                {
                    case Choke _: HandleChoke(); break;
                    case Unchoke _: HandleUnchoke(); break;
                    case Interested _: HandleInterested(); break;
                    case NotInterested _: HandleNotInterested(); break;
                    case Have have: HandleHave(have); break;
                    case Bitfield bitfield: HandleBitfield(bitfield); break;
                }
            }
        }

        private void HandleChoke()
        {
            IsHeChoking = true;

            log.LogInformation("Choke received");
        }

        private void HandleUnchoke()
        {
            IsHeChoking = false;

            log.LogInformation("Unchoke received");
        }

        private void HandleInterested()
        {
            IsHeInterested = true;

            log.LogInformation("Interested received");
        }

        private void HandleNotInterested()
        {
            IsHeInterested = false;

            log.LogInformation("NotInterested received");
        }

        private void HandleHave(Have have)
        {
            PiecesAvailable[have.PieceIndex] = true;

            log.LogInformation("Have received");
        }

        private void HandleBitfield(Bitfield bitfield)
        {
            for (int i = 0; i < bitfield.Bits.Length; i++)
            {
                if (bitfield.Bits[i] is false) continue;
                PiecesAvailable[i] = bitfield.Bits[i];
            }

            log.LogDebug("Bitfield received {Available}/{NumPieces}", PiecesAvailable.Count(b => b), torrent.MetaInfo.NumPieces);
        }

        public void Dispose()
        {
            socket.Dispose();
        }
    }
}
