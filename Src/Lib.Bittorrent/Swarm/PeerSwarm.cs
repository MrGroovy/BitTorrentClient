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
        private IPeerClientFactory peerClientFactory;
        private ILogger log;

        public PeerSwarm(IMessageLoop loop, IPeerClientFactory peerClientFactory, ILogger<PeerSwarm> log)
        {
            this.clients = new List<SwarmEntry>();
            this.loop = loop;
            this.peerClientFactory = peerClientFactory;
            this.log = log;
        }

        public async Task Connect(IPAddress ip, int port, byte[] clientId)
        {
            IPeerClient client = peerClientFactory.Create();

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

        private async Task ReceiveMessages(IPeerClient client)
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
                    else if (message is ChokeMessage choke)
                    {
                        loop.PostChokeReceivedEvent(
                            client.Ip,
                            client.Port,
                            choke);
                    }
                    else if (message is UnchokeMessage unchoke)
                    {
                        loop.PostUnchokeReceivedEvent(
                            client.Ip,
                            client.Port,
                            unchoke);
                    }
                    else if (message is InterestedMessage interested)
                    {
                        loop.PostInterestedReceivedEvent(
                            client.Ip,
                            client.Port,
                            interested);
                    }
                    else if (message is NotInterestedMessage notInterested)
                    {
                        loop.PostNotInterestedReceivedEvent(
                            client.Ip,
                            client.Port,
                            notInterested);
                    }
                    else if (message is HaveMessage have)
                    {
                        loop.PostHaveReceivedEvent(
                            client.Ip,
                            client.Port,
                            have);
                    }
                    else if (message is BitfieldMessage bitfield)
                    {
                        loop.PostBitfieldReceivedEvent(
                            client.Ip,
                            client.Port,
                            bitfield);
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error in ReceiveMessages");
                loop.PostReceiveErrorEvent(client.Ip, client.Port);
            }
        }

        public async Task Close(IPAddress ip, int port)
        {
            if (GetEntry(ip, port) is SwarmEntry entry)
            {
                clients.Remove(entry);
                entry.Client.Dispose();
                await entry.Receiver;
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
            public IPeerClient Client { get; }
            public Task Receiver { get; }

            public SwarmEntry(IPeerClient client, Task receiver)
            {
                Client = client;
                Receiver = receiver;
            }
        }
    }
}
