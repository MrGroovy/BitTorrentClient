using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages.Events
{
    public class KeepAliveReceivedEvent : Message
    {
        private IPAddress ip;
        private int port;
        private ILogger<KeepAliveReceivedEvent> log;

        public KeepAliveReceivedEvent(IPAddress ip, int port, ILogger<KeepAliveReceivedEvent> log)
        {
            this.ip = ip;
            this.port = port;
            this.log = log;
        }

        public override Task Execute(IMessageLoop loop)
        {
            log.LogInformation("KeepAlive received from {ip}:{port}.",
                ip,
                port);

            return Task.CompletedTask;
        }
    }
}
