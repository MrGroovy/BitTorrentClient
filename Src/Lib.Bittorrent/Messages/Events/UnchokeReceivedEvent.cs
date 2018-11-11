using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages.Events
{
    public class UnchokeReceivedEvent : Message
    {
        private readonly IPAddress ip;
        private readonly int port;
        private readonly UnchokeMessage unchoke;
        private readonly ITorrentState state;
        private readonly ILogger<UnchokeReceivedEvent> log;

        public UnchokeReceivedEvent(IPAddress ip, int port, UnchokeMessage unchoke, ITorrentState state, ILogger<UnchokeReceivedEvent> log)
        {
            this.ip = ip;
            this.port = port;
            this.unchoke = unchoke;
            this.state = state;
            this.log = log;
        }

        public override Task Execute(IMessageLoop loop)
        {
            LogUnchokeReceived();
            state.RunInLock(() => state.SetIsHeChoking(ip, port, false));
            return Task.CompletedTask;
        }

        private void LogUnchokeReceived() =>
            log.LogTrace("Unchoke received from {ip}:{port}.",
                ip,
                port);
    }
}