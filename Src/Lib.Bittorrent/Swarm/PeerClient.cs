using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Swarm
{
    public class PeerClient : IDisposable
    {
        public IPAddress Ip { get; private set; }
        public int Port { get; private set; }
        public byte[] ClientId { get; private set; }

        private ITcpClient tcpClient;
        private IMessageLoop loop;
        private ILogger<PeerClient> log;
        private SemaphoreSlim sendLock;
        private Task receiveLoop;

        public PeerClient(ITcpClient tcpClient, IMessageLoop loop, ILogger<PeerClient> log)
        {
            Ip = IPAddress.None;
            Port = 0;
            ClientId = new byte[20];

            this.tcpClient = tcpClient;
            this.loop = loop;
            this.log = log;
            this.sendLock = new SemaphoreSlim(1, 1);
            this.receiveLoop = Task.CompletedTask;
        }

        public async Task Connect(IPAddress ip, int port, TimeSpan timeout)
        {
            Ip = ip;
            Port = port;

            log.LogInformation("Connecting to {ip}:{port}...", Ip, Port);

            Task connectTask = tcpClient.ConnectAsync(Ip, Port);
            Task timeoutTask = Task.Delay(timeout);
            await Task.WhenAny(connectTask, timeoutTask);

            if (!tcpClient.Connected)
            {
                tcpClient.Close();
                throw new Exception($"Unable to establish connection with peer: {Ip}:{Port}.", connectTask.Exception?.InnerException);
            }

            log.LogInformation("Connected to {ip}:{port}.", Ip, Port);

            receiveLoop = ReceiveLoop();
        }

        public void Disconnect()
        {
            tcpClient.Close();
            Task.WaitAll(receiveLoop);
        }

        public async Task SendHandshake(HandshakeMessage handshake)
        {
            log.LogInformation("Sending handshake...");

            byte[] protocolLength = new[] { (byte)handshake.ProtocolString.Length };
            byte[] protocolString = Encoding.UTF8.GetBytes(handshake.ProtocolString);
            byte[] reserved = handshake.Reserved;
            byte[] infoHash = handshake.InfoHash;
            byte[] peerId = handshake.PeerId;

            await Send(
                protocolLength,
                protocolString,
                reserved,
                infoHash,
                peerId);

            log.LogInformation("Handshake send");
        }

        public async Task SendInterested()
        {
            log.LogInformation("Sending Interested...");

            byte[] lengthBytes = IntToBigEndianFourBytes(1);
            byte[] interestedBytes = new[] { (byte)Interested };

            await Send(
                lengthBytes,
                interestedBytes);

            log.LogInformation("Interested send");
        }

        private async Task Send(params IEnumerable<byte>[] parts)
        {
            await Send(parts
                .SelectMany(p => p)
                .ToArray());
        }

        private async Task Send(byte[] bytes)
        {
            await sendLock.WaitAsync();

            try
            {
                await tcpClient.WriteAsync(bytes);
            }
            finally
            {
                sendLock.Release();
            }
        }

        private async Task ReceiveLoop()
        {
            try
            {
                
                await ReceiveHandshake();
                while (true)
                {
                    await ReceiveMessage();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Unhandled error in peer receive loop.");
            }
        }

        /// <remarks>
        /// - If a client receives a handshake with an info_hash that it is not currently
        ///   serving, then the client must drop the connection.
        /// </remarks>
        private async Task ReceiveHandshake()
        {
            int protocolStringLength = await ReceiveByte();
            byte[] protocolString = await ReceiveBytes(protocolStringLength);
            byte[] reserved = await ReceiveBytes(8);
            byte[] infoHash = await ReceiveBytes(20);
            byte[] peerId = await ReceiveBytes(20);

            loop.PostHandshakeReceivedMessage(
                Ip,
                Port,
                new HandshakeMessage(
                    Encoding.UTF8.GetString(protocolString),
                    reserved,
                    infoHash,
                    peerId));
        }

        private const int Choke = 0;
        private const int Unchoke = 1;
        private const int Interested = 2;
        private const int NotInterested = 3;
        private const int Have = 4;
        private const int Bitfield = 5;

        /// <remarks>
        /// - A bitfield of the wrong length is considered an error. Clients should drop the
        ///   connection if they receive bitfields that are not of the correct size, or if
        ///   the bitfield has any of the spare bits set.
        /// </remarks>
        private async Task ReceiveMessage()
        {
            int length = await ReceiveLengthPrefix();

            if (length == 0)
            {
                log.LogInformation("Keep alive received.");
                loop.PostKeepAliveReceivedMessage(Ip, Port);
                return;
            }

            byte[] msgBytes = await ReceiveBytes(length);
            int msgType = msgBytes.First(); // Exception als length 0 is (keep alive bericht)

            if (msgType == Choke)
            {
                log.LogInformation("Choke received.");
            }
            else if (msgType == Unchoke)
            {
                log.LogInformation("Unchoke received.");
            }
            else if (msgType == Interested)
            {
                log.LogInformation("Interested received.");
            }
            else if (msgType == NotInterested)
            {
                log.LogInformation("NotInterested received.");
            }
            else if (msgType == Have)
            {
                log.LogInformation("Have received.");
            }
            else if (msgType == Bitfield)
            {
                log.LogInformation("Bitfield received.");
            }
            else
            {
                log.LogError("Unknown message received.");
            }
        }

        private async Task<int> ReceiveLengthPrefix()
        {
            byte[] lengthBytes = await ReceiveBytes(4);
            int lengthPrefix = BigEndianFourBytesToInt(lengthBytes);
            return lengthPrefix;
        }

        private async Task<byte[]> ReceiveBytes(int numBytes)
        {
            var received = new List<byte>(capacity: numBytes);
            int numBytesRemaining = numBytes;

            while (numBytesRemaining > 0)
            {
                byte[] buffer = new byte[numBytesRemaining];
                int numBytesReceived = await tcpClient.ReadAsync(buffer);

                if (numBytesReceived == 0)
                    throw new Exception("Connection lost? (Zero bytes received)");

                received.AddRange(buffer.Take(numBytesReceived));
                numBytesRemaining -= numBytesReceived;
            }

            return received.ToArray();
        }

        private async Task<byte> ReceiveByte()
        {
            return (await ReceiveBytes(1)).Single();
        }

        private int BigEndianFourBytesToInt(byte[] bytes)
        {
            int i = (bytes[0] << 24)
                | (bytes[1] << 16)
                | (bytes[2] << 8)
                | bytes[3];
            return i;
        }

        private byte[] IntToBigEndianFourBytes(int i)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(i >> 24);
            bytes[1] = (byte)(i >> 16);
            bytes[2] = (byte)(i >> 8);
            bytes[3] = (byte)i;
            return bytes;
        }

        public void Dispose()
        {
            tcpClient.Dispose();
        }
    }
}
