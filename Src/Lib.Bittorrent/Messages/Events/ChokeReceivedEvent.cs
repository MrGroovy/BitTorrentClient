using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages.Events
{
    public class ChokeReceivedEvent : Message
    {
        private readonly IPAddress ip;
        private readonly int port;
        private readonly ChokeMessage choke;
        private readonly ITorrentState state;
        private readonly ILogger<ChokeReceivedEvent> log;

        public ChokeReceivedEvent(IPAddress ip, int port, ChokeMessage choke, ITorrentState state, ILogger<ChokeReceivedEvent> log)
        {
            this.ip = ip;
            this.port = port;
            this.choke = choke;
            this.state = state;
            this.log = log;
        }

        public override Task Execute(IMessageLoop loop)
        {
            LogChokeReceived();
            state.RunInLock(() => state.SetIsHeChoking(ip, port, true));
            return Task.CompletedTask;
        }

        private void LogChokeReceived() =>
            log.LogTrace("Choke received from {ip}:{port}.",
                ip,
                port);
    }
}
