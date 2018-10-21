using Lib.Bittorrent.Swarm;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Bittorrent.UnitTests.Fakes
{
    public class FakeTcpClient : ITcpClient
    {
        public MemoryStream Stream { get; set; }

        public bool Connected => true;

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

        public Task ConnectAsync(IPAddress address, int port)
        {
            return Task.CompletedTask;
        }

        public void Close()
        {
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
            Stream.Dispose();
        }
    }
}
