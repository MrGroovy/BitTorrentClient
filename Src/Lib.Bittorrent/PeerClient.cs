using Microsoft.Extensions.Logging;
using System;
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
        //IsHeChoking
        //IsHeInterested
        //AreWeChoking
        //AreWeInterested
        //PiecesAvailable <PieceIndex, boolean>
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
                Console.WriteLine(ex.ToString());
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
                log.LogInformation(message.GetType().Name);
            }
        }

        public void Dispose()
        {
            socket.Dispose();
        }
    }
}
