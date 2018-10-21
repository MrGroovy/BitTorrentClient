using Lib.Bittorrent.Swarm;
using Lib.Bittorrent.UnitTests.Fakes;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.UnitTests
{
    [TestClass]
    public class PeerClient_ReceiveHandshake_Tests
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
        public async Task WhenPeerIdDoesNotDecodeToStringOfLength20_ThenPeerIsHandledCorrectly()
        {
            // Arrange
            byte[] peerId = new byte[] {
                45, 108, 116, 48, 68,
                50, 48, 45, 89, 218,
                26, 145, 226, 9, 87,
                63, 146, 213, 165, 248 };

            tcpClient.SetUpHandshake(peerId: peerId);
            tcpClient.SetUpComplete();
            
            // Act
            await client.Connect(IPAddress.Loopback, 5000);
            client.Disconnect();

            // Assert
            loop.Verify(m => m.PostHandshakeReceivedMessage(
                It.IsAny<IPAddress>(),
                It.IsAny<int>(),
                It.Is<HandshakeMessage>(h => Enumerable.SequenceEqual(peerId, h.PeerId))));
        }
    }
}
