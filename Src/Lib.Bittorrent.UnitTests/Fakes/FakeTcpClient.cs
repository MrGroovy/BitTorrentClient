﻿using Lib.Bittorrent.Swarm;
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
    public class FakeTcpClient : ITcpClient
    {
        public MemoryStream Stream { get; set; }
        public TimeSpan? ConnectAsyncTimeoutAndThrow { get; set; }
        public bool ConnectAsyncThrowsConnectionRefused { get; set; }
        public bool Connected { get; set; }

        public FakeTcpClient()
        {
            Stream = new MemoryStream();
        }

        public void SetUpTestData(params IEnumerable<byte>[] parts)
        {
            byte[] bytes = parts
                .SelectMany(p => p)
                .ToArray();
            Stream.Write(bytes, 0, bytes.Length);
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

        public void SetUpKeepAlive(byte[] keepAlive)
        {
            SetUpTestData(keepAlive ?? new byte[4]);
        }

        public void SetUpComplete()
        {
            Stream.Position = 0;
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

        public Task<int> ReadAsync(byte[] buffer)
        {
            return Stream.ReadAsync(buffer, 0, buffer.Length);
        }

        public Task WriteAsync(byte[] buffer)
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
