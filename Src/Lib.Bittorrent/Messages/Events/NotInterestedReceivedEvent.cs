using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages.Events
{
    public class NotInterestedReceivedEvent : Message
    {
        private readonly IPAddress ip;
        private readonly int port;
        private readonly NotInterestedMessage notInterested;
        private readonly ITorrentState state;
        private readonly ILogger<NotInterestedReceivedEvent> log;

        public NotInterestedReceivedEvent(IPAddress ip, int port, NotInterestedMessage notInterested, ITorrentState state, ILogger<NotInterestedReceivedEvent> log)
        {
            this.ip = ip;
            this.port = port;
            this.notInterested = notInterested;
            this.state = state;
            this.log = log;
        }

        public override Task Execute(IMessageLoop loop)
        {
            LogNotInterestedReceived();
            state.RunInLock(() => state.SetIsHeInterested(ip, port, false));
            return Task.CompletedTask;
        }

        private void LogNotInterestedReceived() =>
            log.LogTrace("Not interested received from {ip}:{port}.",
                ip,
                port);
    }
}
