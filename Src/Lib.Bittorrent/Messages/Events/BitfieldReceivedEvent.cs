using Lib.Bittorrent.Swarm;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages.Events
{
    public class BitfieldReceivedEvent : Message
    {
        private IPAddress ip;
        private int port;
        private BitfieldMessage bitfield;
        private ILogger<BitfieldReceivedEvent> log;

        public BitfieldReceivedEvent(IPAddress ip, int port, BitfieldMessage bitfield, ILogger<BitfieldReceivedEvent> log)
        {
            this.ip = ip;
            this.port = port;
            this.bitfield = bitfield;
            this.log = log;
        }

        public override Task Execute(IMessageLoop loop)
        {
            log.LogInformation("Bitfield received from {ip}:{port}.",
                ip,
                port);
            return Task.CompletedTask;
        }
    }
}
