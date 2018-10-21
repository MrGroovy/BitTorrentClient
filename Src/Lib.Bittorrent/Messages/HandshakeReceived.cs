using Lib.Bittorrent.Swarm;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages
{
    public class HandshakeReceived : Message
    {
        private IPAddress ip;
        private int port;
        private HandshakeMessage handshake;
        private ILogger<HandshakeReceived> log;

        public HandshakeReceived(IPAddress ip, int port, HandshakeMessage handshake, ILogger<HandshakeReceived> log)
        {
            this.ip = ip;
            this.port = port;
            this.handshake = handshake;
            this.log = log;
        }

        public override Task Execute(IMessageLoop loop)
        {
            log.LogInformation("Handshake received.");

            return Task.CompletedTask;
        }
    }
}
