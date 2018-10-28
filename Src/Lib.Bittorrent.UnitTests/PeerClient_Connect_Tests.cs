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
        private FakeSocket socket;
        private PeerClient client;

        [TestInitialize]
        public void SetUp()
        {
            socket = new FakeSocket();
            client = new PeerClient(
                socket,
                Mock.Of<ILogger<PeerClient>>());
        }

        [TestMethod]
        public async Task WhenConnectionIsRefused_ThenExceptionIsThrown()
        {
            // Arrange
            socket.ConnectAsyncThrowsConnectionRefused = true;

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
            socket.ConnectAsyncTimeoutAndThrow = TimeSpan.FromSeconds(30);

            // Act/Assert
            var ex = await Assert.ThrowsExceptionAsync<SocketException>(() => client.Connect(
                ip: IPAddress.Loopback,
                port: 6881,
                timeout: TimeSpan.FromMilliseconds(50)));
            Assert.AreEqual(SocketError.TimedOut, ex.SocketErrorCode);
        }
    }
}
