using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages
{
    public class KeepAliveReceived : Message
    {
        public IPAddress Ip { get; private set; }
        public int Port { get; private set; }

        public KeepAliveReceived(IPAddress ip, int port)
        {
            Ip = ip;
            Port = port;
        }

        public override Task Execute(MessageLoop loop)
        {
            return Task.CompletedTask;
        }
    }
}
