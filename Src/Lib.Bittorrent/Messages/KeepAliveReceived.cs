using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages
{
    public class KeepAliveReceived : Message
    {
        private IPAddress ip;
        private int port;
        private ILogger<KeepAliveReceived> log;

        public KeepAliveReceived(IPAddress ip, int port, ILogger<KeepAliveReceived> log)
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
