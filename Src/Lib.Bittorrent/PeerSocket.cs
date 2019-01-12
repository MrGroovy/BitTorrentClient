using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lib.Bittorrent
{
    public class PeerSocket : IDisposable, IPeerSocket
    {
        public IPAddress Ip { get; private set; }
        public int Port { get; private set; }

        private ISocket socket;
        private MetaInfo metaInfo;
        private SemaphoreSlim sendLock;
        private ILogger<PeerSocket> log;
        private bool handshakeReceived = false;

        public PeerSocket(ISocket socket, MetaInfo metaInfo, ILogger<PeerSocket> log)
        {
            Ip = IPAddress.None;
            Port = 0;

            this.socket = socket;
            this.metaInfo = metaInfo;
            this.sendLock = new SemaphoreSlim(1, 1);
            this.log = log;
        }

        public async Task Connect(IPAddress ip, int port, TimeSpan timeout)
        {
            Ip = ip;
            Port = port;

            log.LogInformation("Connecting to {ip}:{port}...", Ip, Port);
            await ConnectOrThrow(timeout);
            log.LogInformation("Connected to {ip}:{port}.", Ip, Port);
        }

        private async Task ConnectOrThrow(TimeSpan timeout)
        {
            Task connectTask = socket.ConnectAsync(Ip, Port);
            Task timeoutTask = Task.Delay(timeout);
            await Task.WhenAny(connectTask, timeoutTask);

            if (!socket.Connected)
            {
                socket.Close();
                throw connectTask.Exception?.InnerException is Exception connectTaskEx
                    ? connectTaskEx
                    : new SocketException((int)SocketError.TimedOut);
            }
        }

        public void Close()
        {
            socket.Close();
        }

        #region Sending

        public async Task SendHandshake(string protocolString, byte[] reserved, byte[] infoHash, byte[] peerId)
        {
            log.LogDebug("Sending handshake...");

            byte[] protocolLengthBytes = new[] { (byte)protocolString.Length };
            byte[] protocolStringBytes = Encoding.UTF8.GetBytes(protocolString);

            await Send(
                protocolLengthBytes,
                protocolStringBytes,
                reserved,
                infoHash,
                peerId);

            log.LogDebug("Handshake send");
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
                await socket.SendAsync(bytes);
            }
            finally
            {
                sendLock.Release();
            }
        }

        #endregion

        #region Receiving

        public async Task<Message> ReceiveMessage()
        {
            if (!handshakeReceived)
            {
                int protocolStringLength = await ReceiveByte();
                byte[] protocolString = await ReceiveBytesOrThrow(protocolStringLength);
                byte[] reserved = await ReceiveBytesOrThrow(8);
                byte[] infoHash = await ReceiveBytesOrThrow(20);
                byte[] peerId = await ReceiveBytesOrThrow(20);
                handshakeReceived = true;

                return new Handshake(protocolString, reserved, infoHash, peerId);
            }

            byte[] lengthBytes = await ReceiveBytesOrThrow(4);
            int length = BigEndianFourBytesToInt(lengthBytes);

            if (length == 0)
            {
                return KeepAlive.Instance;
            }

            int messageType = await ReceiveByte();

            if (messageType == 0)
            {
                return Choke.Instance;
            }
            else if (messageType == 1)
            {
                return Unchoke.Instance;
            }
            else if (messageType == 2)
            {
                return Interested.Instance;
            }
            else if (messageType == 3)
            {
                return NotInterested.Instance;
            }
            else if (messageType == 4)
            {
                byte[] haveBytes = await ReceiveBytesOrThrow(4);
                int pieceIndex = BigEndianFourBytesToInt(haveBytes);
                return new Have(pieceIndex, metaInfo);
            }
            else if (messageType == 5)
            {
                byte[] bitFieldBytes = await ReceiveBytesOrThrow(length - 1);
                return new Bitfield(bitFieldBytes, metaInfo);
            }
            else
            {
                throw new NotImplementedException(
                    $"{nameof(ReceiveMessage)} is not fully implemented yet. Unknown message type: {messageType}.");
            }
        }

        private async Task<byte[]> ReceiveBytesOrThrow(int numBytes)
        {
            var received = new List<byte>(capacity: numBytes);
            int numBytesRemaining = numBytes;

            while (numBytesRemaining > 0)
            {
                byte[] buffer = new byte[numBytesRemaining];
                int numBytesReceived = await socket.ReceiveAsync(buffer);

                if (numBytesReceived == 0)
                    throw new SocketException((int)SocketError.ConnectionReset);

                received.AddRange(buffer.Take(numBytesReceived));
                numBytesRemaining -= numBytesReceived;
            }

            return received.ToArray();
        }

        private async Task<byte> ReceiveByte()
        {
            return (await ReceiveBytesOrThrow(1)).Single();
        }

        #endregion

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
            socket.Dispose();
        }
    }
}
