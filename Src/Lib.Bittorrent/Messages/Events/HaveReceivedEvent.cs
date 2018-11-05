using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages.Events
{
    public class HaveReceivedEvent : Message
    {
        private readonly IPAddress ip;
        private readonly int port;
        private readonly HaveMessage have;
        private readonly ITorrentState state;
        private readonly ILogger<HaveReceivedEvent> log;

        public HaveReceivedEvent(IPAddress ip, int port, HaveMessage have, ITorrentState state, ILogger<HaveReceivedEvent> log)
        {
            this.ip = ip;
            this.port = port;
            this.have = have;
            this.state = state;
            this.log = log;
        }

        public override Task Execute(IMessageLoop loop)
        {
            LogHaveReceived();

            try
            {
                state.RunInLock(() => state.MarkPieceAsAvailable(
                    ip,
                    port,
                    have.PieceIndex));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Peer will be closed.");
                loop.PostReceiveErrorEvent(ip, port);
            }

            return Task.CompletedTask;
        }

        private void LogHaveReceived() =>
            log.LogTrace("Have received from {ip}:{port}.",
                ip,
                port);
    }
}
