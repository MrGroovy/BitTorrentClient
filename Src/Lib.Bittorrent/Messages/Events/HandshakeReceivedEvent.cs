using Lib.Bittorrent.Swarm;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages.Events
{
    public class HandshakeReceivedEvent : Message
    {
        private IPAddress ip;
        private int port;
        private HandshakeMessage handshake;
        private ILogger<HandshakeReceivedEvent> log;

        public HandshakeReceivedEvent(IPAddress ip, int port, HandshakeMessage handshake, ILogger<HandshakeReceivedEvent> log)
        {
            this.ip = ip;
            this.port = port;
            this.handshake = handshake;
            this.log = log;
        }

        public override Task Execute(IMessageLoop loop)
        {
            log.LogInformation("Handshake received from {ip}:{port} {peerId}.",
                ip,
                port,
                Encoding.UTF8.GetString(handshake.PeerId));

            return Task.CompletedTask;
        }
    }
}
