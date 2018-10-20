using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Swarm
{
    public class PeerSwarm : IPeerSwarm
    {
        private List<PeerClient> clients;
        private IMessageLooop loop;
        private ILoggerFactory logFactory;

        public PeerSwarm(IMessageLooop loop, ILoggerFactory logFactory)
        {
            this.clients = new List<PeerClient>();
            this.loop = loop;
            this.logFactory = logFactory;
        }

        public async Task Connect(IPAddress ip, int port, byte[] clientId)
        {
            var client = new PeerClient(new SystemTcpClient(), loop, logFactory.CreateLogger<PeerClient>());
            clients.Add(client);

            try
            {
                await client.Connect(ip, port);
            }
            catch
            {
                clients.Remove(client);
                client.Dispose();
                throw;
            }
        }

        public async Task SendHandshake(IPAddress ip, int port, HandshakeMessage handshakeMsg)
        {
            if (GetClient(ip, port) is PeerClient client)
            {
                await client.SendHandshake(handshakeMsg);
                //await client.SendInterested();
            }
        }

        private PeerClient GetClient(IPAddress ip, int port)
        {
            return clients.FirstOrDefault(c => c.Ip == ip && c.Port == port);
        }
    }
}
