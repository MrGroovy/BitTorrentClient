using Lib.Bittorrent.Swarm;
using Lib.Bittorrent.UnitTests.Fakes;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.UnitTests
{
    [TestClass]
    public class PeerClient_KeepAlive_Tests
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
        public async Task WhenAKeepAliveIsReceived_ThenAKeepAliveReceivedMessageIsPostedToTheLoop()
        {
            // Arrange
            tcpClient.SetUpHandshake();
            tcpClient.SetUpKeepAlive(new byte[] { 0, 0, 0, 0 });
            tcpClient.SetUpComplete();
            
            // Act
            await client.Connect(IPAddress.Loopback, 5000, TimeSpan.FromSeconds(4));
            client.Disconnect();

            // Assert
            loop.Verify(m => m.PostKeepAliveReceivedMessage(
                IPAddress.Loopback,
                5000));
        }
    }
}
