using Lib.Bittorrent.Swarm;
using Lib.Bittorrent.UnitTests.Fakes;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lib.Bittorrent.UnitTests
{
    [TestClass]
    public class PeerClient_Connect_Tests
    {
        private FakeTcpClient tcpClient;
        private Mock<IMessageLoop> loop;
        private PeerClient client;

        [TestInitialize]
        public void SetUp()
        {
            tcpClient = new FakeTcpClient();
            loop = new Mock<IMessageLoop>();
            client = new PeerClient(
                tcpClient,
                loop.Object,
                Mock.Of<ILogger<PeerClient>>());
        }

        [TestMethod]
        public async Task WhenConnectionIsRefused_ThenExceptionIsThrown()
        {
            // Arrange
            tcpClient.ConnectAsyncThrowsConnectionRefused = true;

            // Act/Assert
            var ex = await Assert.ThrowsExceptionAsync<SocketException>(() => client.Connect(
                ip: IPAddress.Loopback,
                port: 6881,
                timeout: TimeSpan.FromSeconds(4)));
            Assert.AreEqual(SocketError.ConnectionRefused, ex.SocketErrorCode);
        }

        [TestMethod]
        public async Task WhenConnectionCannotBeEstablishedBeforeTimeout_ThenExceptionIsThrown()
        {
            // Arrange
            tcpClient.ConnectAsyncTimeoutAndThrow = TimeSpan.FromSeconds(30);

            // Act/Assert
            var ex = await Assert.ThrowsExceptionAsync<SocketException>(() => client.Connect(
                ip: IPAddress.Loopback,
                port: 6881,
                timeout: TimeSpan.FromMilliseconds(50)));
            Assert.AreEqual(SocketError.TimedOut, ex.SocketErrorCode);
        }
    }
}
