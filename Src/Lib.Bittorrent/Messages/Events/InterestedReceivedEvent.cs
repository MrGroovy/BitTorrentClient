using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages.Events
{
    public class InterestedReceivedEvent : Message
    {
        private readonly IPAddress ip;
        private readonly int port;
        private readonly InterestedMessage interested;
        private readonly ITorrentState state;
        private readonly ILogger<InterestedReceivedEvent> log;

        public InterestedReceivedEvent(IPAddress ip, int port, InterestedMessage interested, ITorrentState state, ILogger<InterestedReceivedEvent> log)
        {
            this.ip = ip;
            this.port = port;
            this.interested = interested;
            this.state = state;
            this.log = log;
        }

        public override Task Execute(IMessageLoop loop)
        {
            LogInterestedReceived();
            state.RunInLock(() => state.SetIsHeInterested(ip, port, true));
            return Task.CompletedTask;
        }

        private void LogInterestedReceived() =>
            log.LogTrace("Interested received from {ip}:{port}.",
                ip,
                port);
    }
}
