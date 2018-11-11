using Lib.Bittorrent.Swarm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Bittorrent.UnitTests.Fakes
{
    public class FakeSocket : ISocket
    {
        public MemoryStream Stream { get; set; }
        public TimeSpan? ConnectAsyncTimeoutAndThrow { get; set; }
        public bool ConnectAsyncThrowsConnectionRefused { get; set; }
        public bool Connected { get; set; }
        public bool ReceiveInfiniteKeepAlives { get; set; }
        public bool ReadAsyncThrowsAbortedException { get; set; }

        public FakeSocket()
        {
            Stream = new MemoryStream();
        }

        public void SetUpTestData(params IEnumerable<byte>[] parts)
        {
            byte[] bytes = parts
                .SelectMany(p => p)
                .ToArray();
            Stream.Write(bytes, 0, bytes.Length);
            Stream.Position = 0;
        }

        public void SetUpHandshake(
            byte[] protocolLength = null,
            byte[] protocolString = null,
            byte[] reserved = null,
            byte[] infoHash = null,
            byte[] peerId = null)
        {
            SetUpTestData(
                protocolLength ?? new byte[] { 19 },
                protocolString ?? Encoding.UTF8.GetBytes("BitTorrent protocol"),
                reserved ?? new byte[8],
                infoHash ?? new byte[20],
                peerId ?? new byte[20]);
        }

        public void SetUpChoke()
        {
            SetUpTestData(
                new byte[] { 0, 0, 0, 1 },
                new byte[] { 0 });
        }

        public void SetUpUnchoke()
        {
            SetUpTestData(
                new byte[] { 0, 0, 0, 1 },
                new byte[] { 1 });
        }

        public void SetUpKeepAlive(byte[] keepAlive)
        {
            SetUpTestData(keepAlive ?? new byte[4]);
        }

        public void SetUpKeepAlive()
        {
            SetUpTestData(new byte[4]);
        }

        public async Task ConnectAsync(IPAddress address, int port)
        {
            if (ConnectAsyncTimeoutAndThrow != null)
            {
                await Task.Delay(ConnectAsyncTimeoutAndThrow.Value);
                throw new SocketException((int)SocketError.TimedOut);
            }

            if (ConnectAsyncThrowsConnectionRefused)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(20));
                throw new SocketException((int)SocketError.ConnectionRefused);
            }

            Connected = true;
        }

        public void Close()
        {
            Connected = false;
        }

        public async Task<int> ReceiveAsync(byte[] buffer)
        {
            await Task.Delay(50);

            if (ReadAsyncThrowsAbortedException)
            {
                throw new SocketException((int)SocketError.OperationAborted);
            }

            if (ReceiveInfiniteKeepAlives
                && Stream.Position == Stream.Length)
            {
                new byte[Math.Min(buffer.Length, 4)].CopyTo(buffer, 0);
                return Math.Min(buffer.Length, 4);
            }

            return await Stream.ReadAsync(buffer, 0, buffer.Length);
        }

        public Task SendAsync(byte[] buffer)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Connected = false;
            Stream.Dispose();
        }
    }
}
