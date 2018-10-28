using Lib.Bittorrent.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Swarm
{
    public class PeerSwarm : IPeerSwarm
    {
        private List<SwarmEntry> clients;
        private IMessageLoop loop;
        private ILoggerFactory logFactory;
        private ILogger log;

        public PeerSwarm(IMessageLoop loop, ILoggerFactory logFactory, ILogger<PeerSwarm> log)
        {
            this.clients = new List<SwarmEntry>();
            this.loop = loop;
            this.logFactory = logFactory;
            this.log = log;
        }

        public async Task Connect(IPAddress ip, int port, byte[] clientId)
        {
            var client = new PeerClient(new SystemSocket(), logFactory.CreateLogger<PeerClient>());

            try
            {
                await client.Connect(ip, port, TimeSpan.FromSeconds(4));
                clients.Add(new SwarmEntry(client, ReceiveMessages(client)));
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }

        private async Task ReceiveMessages(PeerClient client)
        {
            try
            {
                HandshakeMessage handshake = await client.ReceiveHandshakeMessage();
                loop.PostHandshakeReceivedEvent(
                    client.Ip,
                    client.Port,
                    handshake);

                while (true)
                {
                    ProtocolMessage message = await client.ReceiveMessage();

                    if (message is KeepAliveMessage keepAlive)
                    {
                        loop.PostKeepAliveReceivedEvent(
                            client.Ip,
                            client.Port);
                    }
                    else if (message is BitfieldMessage bitfield)
                    {
                        log.LogInformation("Bitfield received.");
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error in ReceiveMessages");
                loop.PostReceiveErrorEvent(client.Ip, client.Port);
            }
        }

        public void Remove(IPAddress ip, int port)
        {
            if (GetEntry(ip, port) is SwarmEntry entry)
            {
                clients.Remove(entry);
                entry.Client.Close();
            }
        }

        public async Task SendHandshake(IPAddress ip, int port, HandshakeMessage handshakeMsg)
        {
            if (GetEntry(ip, port) is SwarmEntry entry)
            {
                await entry.Client.SendHandshake(handshakeMsg);
            }
        }

        private SwarmEntry GetEntry(IPAddress ip, int port)
        {
            return clients.FirstOrDefault(e => e.Client.Ip == ip && e.Client.Port == port);
        }

        private class SwarmEntry
        {
            public PeerClient Client { get; }
            public Task Receiver { get; }

            public SwarmEntry(PeerClient client, Task receiver)
            {
                Client = client;
                Receiver = receiver;
            }
        }
    }
}
